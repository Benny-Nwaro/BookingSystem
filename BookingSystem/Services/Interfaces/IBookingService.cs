using BookingSystem.Models.DTOs;
using BookingSystem.Models;
using BookingSystem.Models.Common;
namespace BookingSystem.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<AvailableRoomResponse>> SearchAvailabilityAsync(AvailabilityRequest request);
    Task CreateBookingAsync(Guid userId, CreateBookingRequest request);
    Task CancelBookingAsync(Guid bookingId, Guid userId);
    Task<Models.Booking> CreateHoldAsync(Guid userId, CreateHoldRequest request);
    Task CleanupExpiredHoldsAsync();
    Task<PagedResponse<BookingListItem>> GetUserBookingsAsync(
        Guid userId, BookingQuery query);

    Task<PagedResponse<BookingListItem>> GetAllBookingsAsync(
        BookingQuery query);
}
