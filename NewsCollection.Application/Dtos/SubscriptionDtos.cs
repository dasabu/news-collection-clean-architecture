namespace NewsCollection.Application.Dtos;

public record class SubscriptionDto(
    int CategoryId,
    string CategoryName,
    bool IsActive,
    DateTime? LastNotified
);

public record class CreateSubscriptionDto(
    int CategoryId   
);

