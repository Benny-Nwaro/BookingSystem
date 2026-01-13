namespace BookingSystem.Models.Admin;

public class RoomRequest
{
    public Guid HotelId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
}
