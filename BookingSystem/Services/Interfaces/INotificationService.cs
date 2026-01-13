using BookingSystem.Models;

namespace BookingSystem.Services.Interfaces;

public interface INotificationService
{
    Task SendBookingConfirmationAsync(User user, Booking booking);
    Task SendBookingCancellationAsync(User user, Booking booking);
}
