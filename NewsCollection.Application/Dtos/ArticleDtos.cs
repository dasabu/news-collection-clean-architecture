namespace NewsCollection.Application.Dtos;

public record class ArticleDto(
    int Id,
    string Headline,
    string Summary,
    string Url,
    DateTime PublicationDate,
    int CategoryId,
    string CategoryName
);

public record CreateArticleDto
(
    string Headline,
    string Summary,
    string Content,
    string Url,
    DateTime PublicationDate,
    int CategoryId
);

public record UpdateArticleDto
(
    string Headline,
    string Summary,
    string Url,
    DateTime PublicationDate,
    int CategoryId
);

public record AddArticleToCollectionDto(int ArticleId);

