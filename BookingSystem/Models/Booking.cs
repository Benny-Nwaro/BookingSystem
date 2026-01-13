using BookingSystem.Enums;
namespace BookingSystem.Models;

public class Booking
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Room? Room { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime? HoldExpiresAt { get; set; }

}