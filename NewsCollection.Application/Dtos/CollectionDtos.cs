namespace NewsCollection.Application.Dtos;

public record class CollectionDto(
    int Id,
    string Name,
    string? Description,
    int ArticleCount,
    DateOnly UpdatedAt
);

public record class CreateCollectionDto(
    string Name,
    string? Description
);

public record UpdateCollectionDto(
    string Name,
    string? Description
);