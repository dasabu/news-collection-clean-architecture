using System;

namespace NewsCollection.Domain.Entities;

public class UserSubscription
{
    public int UserId { get; set; } // FK to User
    public int CategoryId { get; set; } // FK to Category
    public bool IsActive { get; set; } = true; // Toggle on/off
    public string Frequency { get; set; } = "daily"; // "daily" or "weekly"
    public DateTime? LastNotified { get; set; } // Last time sending noti
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}