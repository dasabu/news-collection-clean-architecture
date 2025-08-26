namespace NewsCollection.Application.Dtos;

public record class CollectionDto(
    int Id,
    string Name,
    string? Description,
    int ArticleCount,
    DateOnly UpdatedAt
);

public record CreateCollectionDto(
    string Name,
    string? Description
);
