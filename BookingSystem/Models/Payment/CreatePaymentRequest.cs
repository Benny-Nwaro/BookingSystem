namespace BookingSystem.Models.Payment;

public class CreatePaymentRequest
{
    public Guid HoldId { get; set; }
    public bool SimulateSuccess { get; set; }
}
