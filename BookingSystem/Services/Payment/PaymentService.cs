using BookingSystem.Data;
using BookingSystem.Enums;
using BookingSystem.Exceptions;
using BookingSystem.Models;
using BookingSystem.Models.Payment;
using BookingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Services;

public class PaymentService : IPaymentService
{
    private readonly BookingDbContext _context;
    private readonly INotificationService _notificationService;

    public PaymentService(
        BookingDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(Guid userId, CreatePaymentRequest request)
    {
        var hold = await _context.Holds
            .Include(h => h.Room!)
            .ThenInclude(r => r.Hotel!)
            .FirstOrDefaultAsync(h =>
                h.Id == request.HoldId &&
                h.UserId == userId);

        if (hold == null)
            throw new NotFoundException("Reservation hold not found");

        if (hold.HoldExpiresAt < DateTime.UtcNow)
            throw new BadRequestException("Reservation hold has expired");

        using var transaction = await _context.Database.BeginTransactionAsync();

        var nights =
            (hold.CheckOutDate - hold.CheckInDate).Days;

        var amount = nights * hold.Room!.PricePerNight;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            HoldId = hold.Id,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // SIMULATED PAYMENT RESULT
        if (!request.SimulateSuccess)
        {
            payment.Status = PaymentStatus.Failed;
            await _context.SaveChangesAsync();

            return new PaymentResponse
            {
                PaymentId = payment.Id,
                Status = PaymentStatus.Failed,
                Message = "Payment failed"
            };
        }

        // PAYMENT SUCCESS → CREATE BOOKING
        payment.Status = PaymentStatus.Succeeded;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = hold.RoomId,
            UserId = userId,
            CheckInDate = hold.CheckInDate,
            CheckOutDate = hold.CheckOutDate,
            Status = BookingStatus.Reserved
        };

        _context.Bookings.Add(booking);
        _context.Holds.Remove(hold);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            await _notificationService
                .SendBookingConfirmationAsync(user, booking);
        }

        return new PaymentResponse
        {
            PaymentId = payment.Id,
            Status = PaymentStatus.Succeeded,
            Message = "Payment successful. Booking confirmed."
        };
    }
}
