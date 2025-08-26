using System;

namespace NewsCollection.Domain.Entities;

public class Article
{
    public int Id { get; set; }
    public required string Headline { get; set; }
    public required string Summary { get; set; }
    public required string Url { get; set; }
    public required DateOnly PublicationDate { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // many-to-many
    public List<Collection> Collections { get; set; } = new();
}
