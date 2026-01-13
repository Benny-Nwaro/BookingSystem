namespace BookingSystem.Models.DTOs;

public class CreateBookingRequest
{
    public Guid RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}
