namespace BookingSystem.Models.Admin;

public class BookingFilterRequest
{
    public Guid
    ? HotelId { get; set; }
    public Guid
    ? RoomId { get; set; }
    public Guid
    ? UserId { get; set; }
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
