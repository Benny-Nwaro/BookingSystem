using BookingSystem.Data;
using BookingSystem.Enums;
using BookingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Models.DTOs;
using BookingSystem.Models.Common;
using BookingSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using BookingSystem.Exceptions;

namespace BookingSystem.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _context;
    private readonly INotificationService _notificationService;

    public BookingService(BookingDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<AvailableRoomResponse>> SearchAvailabilityAsync(
        AvailabilityRequest request)
    {
        var rooms = await _context.Rooms
            .Where(r => r.HotelId == request.HotelId)
            .Where(r =>
                !r.Bookings.Any(b =>
                    b.Status == BookingStatus.Reserved &&
                    b.CheckInDate < request.CheckOutDate &&
                    b.CheckOutDate > request.CheckInDate
                )
            )
            .Select(r => new AvailableRoomResponse
            {
                RoomId = r.Id,
                RoomNumber = r.RoomNumber,
                PricePerNight = r.PricePerNight
            })
            .ToListAsync();

        return rooms;
    }

    public async Task CreateBookingAsync(Guid userId, CreateBookingRequest request)
    {
        if (request.CheckInDate >= request.CheckOutDate)
            throw new BadRequestException("Invalid date range");

        using var transaction = await _context.Database.BeginTransactionAsync();

        var room = await _context.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId);

        if (room == null)
            throw new NotFoundException("Room not found");

        var isOverlapping = room.Bookings.Any(b =>
            b.Status == BookingStatus.Reserved &&
            b.CheckInDate < request.CheckOutDate &&
            b.CheckOutDate > request.CheckInDate
        );

        if (isOverlapping)
            throw new BadRequestException("Room is not available");

        var booking = new Models.Booking
        {
            RoomId = request.RoomId,
            UserId = userId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Status = BookingStatus.Reserved
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
        await _notificationService.SendBookingConfirmationAsync(
            await _context.Users.FindAsync(userId) ?? throw new ApplicationException("User not found"),
            booking);
    }

    public async Task CancelBookingAsync(Guid bookingId, Guid userId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        booking.Status = BookingStatus.Cancelled;
        await _context.SaveChangesAsync();
        await _notificationService.SendBookingCancellationAsync(
            await _context.Users.FindAsync(userId) ?? throw new ApplicationException("User not found"),
            booking);
    }

    public async Task<Models.Booking> CreateHoldAsync(Guid userId, CreateHoldRequest request)
    {
        if (request.CheckInDate >= request.CheckOutDate)
            throw new BadRequestException("Invalid date range");

        using var transaction = await _context.Database.BeginTransactionAsync();

        var room = await _context.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId);

        if (room == null)
            throw new NotFoundException("Room not found");

        // Remove expired holds first
        await CleanupExpiredHoldsAsync();

        // Check for overlapping bookings or holds
        var isOverlapping = room.Bookings.Any(b =>
            (b.Status == BookingStatus.Reserved ||
            b.Status == BookingStatus.Hold) &&
            b.CheckInDate < request.CheckOutDate &&
            b.CheckOutDate > request.CheckInDate
        );

        if (isOverlapping)
            throw new BadRequestException("Room is not available");

        var holdExpiration = DateTime.UtcNow.AddMinutes(request.HoldMinutes);

        var bookingHold = new Models.Booking
        {
            RoomId = request.RoomId,
            UserId = userId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Status = BookingStatus.Hold,
            HoldExpiresAt = holdExpiration
        };

        _context.Bookings.Add(bookingHold);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return bookingHold;
    }

    public async Task CleanupExpiredHoldsAsync()
    {
        var expiredHolds = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Hold &&
                        b.HoldExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        if (expiredHolds.Any())
        {
            _context.Bookings.RemoveRange(expiredHolds);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PagedResponse<BookingListItem>> GetUserBookingsAsync(
    Guid userId, BookingQuery query)
{
    var bookingsQuery = _context.Bookings
        .Where(b => b.UserId == userId)
        .Include(b => b.Room!)
        .ThenInclude(r => r.Hotel!)
        .AsQueryable();

    bookingsQuery = ApplyFilters(bookingsQuery, query);

    return await PaginateAsync(bookingsQuery, query);
}

public async Task<PagedResponse<BookingListItem>> GetAllBookingsAsync(
    BookingQuery query)
{
    var bookingsQuery = _context.Bookings
        .Include(b => b.Room!)
        .ThenInclude(r => r.Hotel!)
        .AsQueryable();

    bookingsQuery = ApplyFilters(bookingsQuery, query);

    return await PaginateAsync(bookingsQuery, query);
}

private IQueryable<Models.Booking> ApplyFilters(
    IQueryable<Models.Booking> query,
    BookingQuery filter)
{
    if (filter.Status.HasValue)
        query = query.Where(b => b.Status == filter.Status);

    if (filter.FromDate.HasValue)
        query = query.Where(b => b.CheckInDate >= filter.FromDate);

    if (filter.ToDate.HasValue)
        query = query.Where(b => b.CheckOutDate <= filter.ToDate);

    return query;
}

private async Task<PagedResponse<BookingListItem>> PaginateAsync(
    IQueryable<Models.Booking> query,
    BookingQuery request)
{
    var totalCount = await query.CountAsync();

    var items = await query
        .OrderByDescending(b => b.CheckInDate)
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(b => new BookingListItem
        {
            BookingId = b.Id,
            HotelName = b.Room!.Hotel!.Name,
            RoomNumber = b.Room.RoomNumber,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            Status = b.Status
        })
        .ToListAsync();

    return new PagedResponse<BookingListItem>
    {
        Page = request.Page,
        PageSize = request.PageSize,
        TotalCount = totalCount,
        Items = items
    };
}

}
