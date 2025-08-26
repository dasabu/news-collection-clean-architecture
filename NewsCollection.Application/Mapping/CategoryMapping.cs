using System;
using NewsCollection.Application.Dtos;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Mapping;
public static class CategoryMapping
{
    public static CategoryDto ToDto(this Category category)
    {
        return new(category.Id, category.Name);
    }
}