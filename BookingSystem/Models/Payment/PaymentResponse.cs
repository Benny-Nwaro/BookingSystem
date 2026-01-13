using BookingSystem.Enums;

namespace BookingSystem.Models.Payment;

public class PaymentResponse
{
    public Guid PaymentId { get; set; }
    public PaymentStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
}
