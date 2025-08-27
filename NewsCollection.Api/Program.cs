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
using Scalar.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using NewsCollection.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// DbContext
var connString = builder.Configuration.GetConnectionString("NewsCollectionDb");
builder.Services.AddNpgsql<NewsCollectionContext>(connString);

// Hangfire
// https://stackoverflow.com/questions/78518867/how-to-fix-usepostgresqlstorage-is-obsolete-will-be-removed-in-2-0-in-hangfir
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

// Add Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

app.MapControllers();

// Schedule Hangfire jobs
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<NewsSyncJob>(
        "news-sync",
        job => job.ExecuteAsync(),
        "*/2 * * * * *"); // Every 5 mins (for testing)
        // "0 0 6,12,18,0 * * *"); // 6:00, 12:00, 18:00, 00:00
    recurringJobManager.AddOrUpdate<DigestEmailJob>(
        "digest-email",
        job => job.ExecuteAsync(),
        "1 */2 * * * *"); // Every 5 mins after sync 1 min (for testing)
                          // "0 8 * * *"); // 8:00 
    recurringJobManager.AddOrUpdate<NotificationJob>(
        "notification-email",
        job => job.ExecuteAsync(),
        "*/1 * * * *"); // Every 2 mins (for testing)
                        // "0 0 * * * *"); // Every hour
    recurringJobManager.AddOrUpdate<SummaryJob>(
        "summary-email",
        job => job.ExecuteAsync(),
        "2 */2 * * * *"); // Every 5 mins, offset 2 min with sync (for testing)
        // "0 20 * * 0"); // 20:00
}

app.Run();
