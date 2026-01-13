using BookingSystem.Models;
using BookingSystem.Services.Interfaces;

namespace BookingSystem.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IWhatsAppSender _whatsAppSender;

    public NotificationService(
        IEmailSender emailSender,
        IWhatsAppSender whatsAppSender)
    {
        _emailSender = emailSender;
        _whatsAppSender = whatsAppSender;
    }

    public async Task SendBookingConfirmationAsync(
        User user, Booking booking)
    {
        var subject = "Booking Confirmation";
        var message =
            $"Your booking is confirmed from {booking.CheckInDate:d} " +
            $"to {booking.CheckOutDate:d}.";

        await _emailSender.SendAsync(user.Email, subject, message);

        await _whatsAppSender.SendAsync(
            "+1234567890",
            $" Booking confirmed ({booking.Id})");
    }

    public async Task SendBookingCancellationAsync(
        User user, Booking booking)
    {
        var subject = "Booking Cancelled";
        var message =
            $"Your booking from {booking.CheckInDate:d} " +
            $"to {booking.CheckOutDate:d} has been cancelled.";

        await _emailSender.SendAsync(user.Email, subject, message);

        await _whatsAppSender.SendAsync(
            "+1234567890",
            $" Booking cancelled ({booking.Id})");
    }
}
