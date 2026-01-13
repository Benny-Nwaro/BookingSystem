using BookingSystem.Enums;

namespace BookingSystem.Models.Payment;

public class Payment
{
    public Guid Id { get; set; }
    public Guid HoldId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
