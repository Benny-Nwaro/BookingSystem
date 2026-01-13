using BookingSystem.Models.Payment;

namespace BookingSystem.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(
        Guid userId,
        CreatePaymentRequest request);
}
