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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure these data will exist when migration is completed
        modelBuilder.Entity<Category>().HasData(
            new { Id = 1, Name = "Technology" },
            new { Id = 2, Name = "Sports" },
            new { Id = 3, Name = "Politics" },
            new { Id = 4, Name = "Health" },
            new { Id = 5, Name = "Entertainment" }
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
    }
}
