using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewsCollection.Application.Common;
using NewsCollection.Application.Interfaces;
using NewsCollection.Application.Services;
using NewsCollection.Infrastructure.Data;
using NewsCollection.Infrastructure.Providers;
using NewsCollection.Infrastructure.Repositories;
using NewsCollection.Infrastructure.Jobs;
using NewsCollection.Api.Filters;
using Scalar.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "AllowLocal3000";

// Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ResponseWrapperFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// DbContext
var connString = builder.Configuration.GetConnectionString("NewsCollectionDb");
builder.Services.AddNpgsql<NewsCollectionContext>(connString);

// Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connString)));
builder.Services.AddHangfireServer();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailLogRepository, EmailLogRepository>();

// Providers
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<INewsApiProvider, NewsApiProvider>();

// Add HttpClientFactory
builder.Services.AddHttpClient();

// Register JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Token"]!))
        };
    });
builder.Services.AddAuthorization();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors(MyAllowSpecificOrigins);

// Add Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

app.UseRouting();

app.UseAuthentication(); // Add this to enable JWT authentication
app.UseAuthorization(); // Add this to enable authorization for [Authorize] attributes

app.MapControllers();

// Schedule Hangfire jobs
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<NewsSyncJob>(
        "news-sync",
        job => job.ExecuteAsync(),
        "*/2 * * * * *"); // Every 5 mins (for testing)
    recurringJobManager.AddOrUpdate<DigestEmailJob>(
        "digest-email",
        job => job.ExecuteAsync(),
        "1 */2 * * * *"); // Every 5 mins after sync 1 min (for testing)
    recurringJobManager.AddOrUpdate<NotificationJob>(
        "notification-email",
        job => job.ExecuteAsync(),
        "*/1 * * * *"); // Every 2 mins (for testing)
    recurringJobManager.AddOrUpdate<SummaryJob>(
        "summary-email",
        job => job.ExecuteAsync(),
        "2 */2 * * * *"); // Every 5 mins, offset 2 min with sync (for testing)
}

app.Run();