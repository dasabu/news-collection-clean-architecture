using System;

namespace NewsCollection.Domain.Entities;

public class Collection
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // soft-delete
    public bool IsDeleted { get; set; } = false;

    // many-to-many
    public List<Article> Articles { get; set; } = new();
}
