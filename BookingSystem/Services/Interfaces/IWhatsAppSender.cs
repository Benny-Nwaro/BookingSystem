namespace BookingSystem.Services.Interfaces;

public interface IWhatsAppSender
{
    Task SendAsync(string phoneNumber, string message);
}
