namespace NewsCollection.Application.Dtos;

public record class CategoryDto(int Id, string Name);

public record class CreateCategoryDto(string Name);

public record class UpdateCategoryDto(string Name);