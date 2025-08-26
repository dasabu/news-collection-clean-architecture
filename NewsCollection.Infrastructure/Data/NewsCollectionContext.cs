using System;
using Microsoft.EntityFrameworkCore;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Infrastructure.Data;

public class NewsCollectionContext(DbContextOptions<NewsCollectionContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<CollectionArticle> CollectionArticles => Set<CollectionArticle>();

    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure these data will exist when migration is completed
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Technology" },
            new Category { Id = 2, Name = "Business" },
            new Category { Id = 3, Name = "Health" },
            new Category { Id = 4, Name = "Sports" },
            new Category { Id = 5, Name = "Science" }
        );

        // // Create associative table of Collection and Article tables
        // modelBuilder.Entity<Collection>()
        //             .HasMany(c => c.Articles)
        //             .WithMany(a => a.Collections)
        //             .UsingEntity(j => j.ToTable("CollectionArticles"));

        modelBuilder.Entity<CollectionArticle>()
            .HasKey(ca => new { ca.CollectionId, ca.ArticleId });

        modelBuilder.Entity<Collection>()
            .HasMany(c => c.Articles)
            .WithMany(a => a.Collections)
            .UsingEntity<CollectionArticle>(
                j => j.HasOne(ca => ca.Article).WithMany().HasForeignKey(ca => ca.ArticleId),
                j => j.HasOne(ca => ca.Collection).WithMany().HasForeignKey(ca => ca.CollectionId),
                j => j.ToTable("CollectionArticles")
            );

        // global query filter for soft-delete
        modelBuilder.Entity<Collection>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<CollectionArticle>()
            .HasQueryFilter(ca => !ca.IsDeleted);

        // Cấu hình UserSubscription
        modelBuilder.Entity<UserSubscription>()
            .HasKey(us => new { us.UserId, us.CategoryId });
        modelBuilder.Entity<UserSubscription>()
            .HasOne(us => us.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(us => us.UserId);
        modelBuilder.Entity<UserSubscription>()
            .HasOne(us => us.Category)
            .WithMany(c => c.Subscriptions)
            .HasForeignKey(us => us.CategoryId);
        modelBuilder.Entity<UserSubscription>()
            .Property(us => us.IsActive)
            .HasDefaultValue(true);

        // Cấu hình EmailLog
        modelBuilder.Entity<EmailLog>()
            .HasOne(el => el.User)
            .WithMany() // Không cần navigation property ngược nếu không dùng
            .HasForeignKey(el => el.UserId);

        // Cấu hình default CreatedAt
        modelBuilder.Entity<CollectionArticle>()
            .Property(ca => ca.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Cấu hình default FetchedAt
        modelBuilder.Entity<Article>()
            .Property(a => a.FetchedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Cấu hình default SubscriptionFrequency
        modelBuilder.Entity<User>()
            .Property(u => u.SubscriptionFrequency)
            .HasDefaultValue("daily");
    }
}
