using BookingSystem.Data;
using BookingSystem.Enums;
using BookingSystem.Models.Admin;
using BookingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Services;

public class AdminService : IAdminService
{
    private readonly BookingDbContext _context;

    public AdminService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateHotelAsync(HotelRequest request)
    {
        var hotel = new Models.Hotel
        {
            Name = request.Name,
            Address = request.City
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        return hotel.Id;
    }

    public async Task UpdateHotelAsync(Guid hotelId, HotelRequest request)
    {
        var hotel = await _context.Hotels.FindAsync(hotelId);
        if (hotel == null) throw new ApplicationException("Hotel not found");

        hotel.Name = request.Name;
        hotel.Address = request.City;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteHotelAsync(Guid hotelId)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == hotelId);

        if (hotel == null) throw new ApplicationException("Hotel not found");

        if (hotel.Rooms.Any())
            throw new ApplicationException("Cannot delete hotel with rooms");

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> CreateRoomAsync(RoomRequest request)
    {
        var hotel = await _context.Hotels.FindAsync(request.HotelId);
        if (hotel == null) throw new ApplicationException("Hotel not found");

        var room = new Models.Room
        {
            HotelId = request.HotelId,
            RoomNumber = request.RoomNumber,
            PricePerNight = request.PricePerNight
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return room.Id;
    }

    public async Task UpdateRoomAsync(Guid roomId, RoomRequest request)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null) throw new ApplicationException("Room not found");

        room.HotelId = request.HotelId;
        room.RoomNumber = request.RoomNumber;
        room.PricePerNight = request.PricePerNight;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoomAsync(Guid roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (room == null) throw new ApplicationException("Room not found");

        if (room.Bookings.Any(b => b.Status == BookingStatus.Reserved))
            throw new ApplicationException("Cannot delete room with active bookings");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<object>> GetBookingsAsync(BookingFilterRequest request)
    {
        var query = _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r!.Hotel)
            .Include(b => b.User)
            .AsQueryable();

        if (request.HotelId.HasValue)
            query = query.Where(b => b.Room!.HotelId == request.HotelId.Value);
        if (request.RoomId.HasValue)
            query = query.Where(b => b.RoomId == request.RoomId.Value);
        if (request.UserId.HasValue)
            query = query.Where(b => b.UserId == request.UserId.Value);

        var skip = (request.Page - 1) * request.PageSize;

        var bookings = await query
            .OrderByDescending(b => b.CheckInDate)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(b => new
            {
                b.Id,
                b.UserId,
                UserEmail = b.User!.Email,
                b.RoomId,
                RoomNumber = b.Room!.RoomNumber,
                HotelName = b.Room!.Hotel!.Name,
                b.CheckInDate,
                b.CheckOutDate,
                b.Status
            })
            .ToListAsync();

        return bookings;
    }
}
