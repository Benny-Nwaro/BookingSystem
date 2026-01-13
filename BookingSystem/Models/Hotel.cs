using BookingSystem.Enums;
namespace BookingSystem.Models;

public class Hotel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public ICollection<Room> Rooms { get; set; } = new List<Room>();

}