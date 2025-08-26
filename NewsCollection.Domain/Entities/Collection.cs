using System;

namespace NewsCollection.Domain.Entities;

public class Collection
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public DateOnly CreatedAt { get; set; }
    public DateOnly UpdatedAt { get; set; }

    // many-to-many
    public List<Article> Articles { get; set; } = new();
}
