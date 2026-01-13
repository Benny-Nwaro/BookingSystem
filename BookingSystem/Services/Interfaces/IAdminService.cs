using BookingSystem.Models.Admin;

namespace BookingSystem.Services.Interfaces;

public interface IAdminService
{
    // Hotel management
    Task<Guid> CreateHotelAsync(HotelRequest request);
    Task UpdateHotelAsync(Guid hotelId, HotelRequest request);
    Task DeleteHotelAsync(Guid hotelId);

    // Room management
    Task<Guid> CreateRoomAsync(RoomRequest request);
    Task UpdateRoomAsync(Guid roomId, RoomRequest request);
    Task DeleteRoomAsync(Guid roomId);

    // Booking viewing
    Task<IEnumerable<object>> GetBookingsAsync(BookingFilterRequest request);
}
