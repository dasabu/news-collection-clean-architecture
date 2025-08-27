using System;

namespace NewsCollection.Domain.Entities;

public class Article
{
    public int Id { get; set; }
    public required string Headline { get; set; }
    public required string Summary { get; set; }
    public required string Url { get; set; }
    public required DateTime PublicationDate { get; set; }
    public DateTime FetchedAt { get; set; } // sync time from NewsAPI
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // many-to-many
    public List<Collection> Collections { get; set; } = new();
}
