using System;

namespace NewsCollection.Domain.Entities;

public class EmailLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty; // "digest" or "notification" or "summary"
    public DateTime SentAt { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public User User { get; set; } = null!;
}
