using BookingSystem.Services.Interfaces;

namespace BookingSystem.Services;

public class WhatsAppSender : IWhatsAppSender
{
    public Task SendAsync(string phoneNumber, string message)
    {
        Console.WriteLine(" WHATSAPP MESSAGE SENT");
        Console.WriteLine($"To: {phoneNumber}");
        Console.WriteLine(message);
        Console.WriteLine("------------------------");

        return Task.CompletedTask;
    }
}
