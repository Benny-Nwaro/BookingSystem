namespace BookingSystem.Models.DTOs;

public class CreateHoldRequest
{
    public Guid RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public int HoldMinutes { get; set; } = 15;
}
