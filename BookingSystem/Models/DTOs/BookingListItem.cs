using BookingSystem.Enums;

namespace BookingSystem.Models;

public class BookingListItem
{
    public Guid BookingId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public BookingStatus Status { get; set; }
}
