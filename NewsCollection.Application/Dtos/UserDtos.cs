namespace NewsCollection.Application.Dtos;

public record class UserDto(
    int Id,
    string Email,
    string Name
);

public record class UpdateUserFrequencyDto(
    string Frequency = "daily",
    int? CategoryId = null
);