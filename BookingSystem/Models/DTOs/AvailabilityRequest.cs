namespace BookingSystem.Models.DTOs;

public class AvailabilityRequest
{
    public Guid HotelId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}
