namespace BookingSystem.Models.DTOs;

public class AvailableRoomResponse
{
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
}
