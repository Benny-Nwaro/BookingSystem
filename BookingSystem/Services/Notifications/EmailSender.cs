using BookingSystem.Services.Interfaces;

namespace BookingSystem.Services;

public class EmailSender : IEmailSender
{
    public Task SendAsync(string to, string subject, string body)
    {
        Console.WriteLine(" EMAIL SENT");
        Console.WriteLine($"To: {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine(body);
        Console.WriteLine("------------------------");

        return Task.CompletedTask;
    }
}
