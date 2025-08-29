using System.ComponentModel.DataAnnotations;

namespace NewsCollection.Application.Dtos;

public record class CollectionDto(
    int Id,
    [Required]
    string Name,
    string? Description,
    int ArticleCount,
    DateTime UpdatedAt
);

public record class CreateCollectionDto(
    [Required]
    string Name,
    string? Description
);

public record UpdateCollectionDto(
    [Required]
    string Name,
    string? Description
);