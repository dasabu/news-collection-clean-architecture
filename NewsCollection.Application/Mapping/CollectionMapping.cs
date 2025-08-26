using NewsCollection.Application.Dtos;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Mapping;

public static class CollectionMapping
{
    public static CollectionDto ToDto(this Collection collection)
    {
        return new(
            collection.Id,
            collection.Name,
            collection.Description,
            collection.Articles.Count,
            collection.UpdatedAt
        );
    }
        
}