using System;
using NewsCollection.Application.Dtos;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Mapping;

public static class ArticleMapping
{
    public static ArticleDto ToDto(this Article article)
    {
        return new(
            article.Id,
            article.Headline,
            article.Summary,
            article.Url,
            article.PublicationDate,
            article.CategoryId,
            article.Category?.Name ?? ""
        );
    }
}
