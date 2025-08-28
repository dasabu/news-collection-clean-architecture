namespace NewsCollection.Application.Dtos;

public record class SubscriptionDto(
    int CategoryId,
    string CategoryName,
    bool IsActive,
    DateTime? LastNotified,
    string Frequency
);

public record class CreateSubscriptionDto(
    int CategoryId,
    string? Frequency = null
);

public record class BatchSubscriptionDto(
    List<SubscriptionRequestDto> Subscriptions
);

public record class SubscriptionRequestDto(
    int CategoryId,
    bool IsActive,
    string Frequency
);