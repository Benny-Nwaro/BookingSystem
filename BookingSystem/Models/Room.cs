namespace BookingSystem.Models;

public class Room
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public string RoomNumber { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
