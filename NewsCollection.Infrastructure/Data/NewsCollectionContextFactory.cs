using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NewsCollection.Infrastructure.Data;

public class NewsCollectionContextFactory : IDesignTimeDbContextFactory<NewsCollectionContext>
{
    public NewsCollectionContext CreateDbContext(string[] args)
    {
        // Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<NewsCollectionContext>();
        var connectionString = configuration.GetConnectionString("NewsCollectionDb");
        optionsBuilder.UseNpgsql(connectionString);

        return new NewsCollectionContext(optionsBuilder.Options);
    }
}