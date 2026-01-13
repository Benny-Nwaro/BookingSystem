using BookingSystem.Models.Admin;
using BookingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;

namespace BookingSystem.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IBookingService _bookingService;

    public AdminController(IAdminService adminService, IBookingService bookingService)
    {
        _adminService = adminService;
        _bookingService = bookingService;
    }

    [HttpPost("hotels")]
    public async Task<IActionResult> CreateHotel(HotelRequest request)
    {
        var id = await _adminService.CreateHotelAsync(request);
        return Ok(new { HotelId = id });
    }

    [HttpPut("hotels/{id}")]
    public async Task<IActionResult> UpdateHotel(Guid id, HotelRequest request)
    {
        await _adminService.UpdateHotelAsync(id, request);
        return Ok("Hotel updated successfully");
    }

    [HttpDelete("hotels/{id}")]
    public async Task<IActionResult> DeleteHotel(Guid id)
    {
        await _adminService.DeleteHotelAsync(id);
        return Ok("Hotel deleted successfully");
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom(RoomRequest request)
    {
        var id = await _adminService.CreateRoomAsync(request);
        return Ok(new { RoomId = id });
    }

    [HttpPut("rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(Guid id, RoomRequest request)
    {
        await _adminService.UpdateRoomAsync(id, request);
        return Ok("Room updated successfully");
    }

    [HttpDelete("rooms/{id}")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        await _adminService.DeleteRoomAsync(id);
        return Ok("Room deleted successfully");
    }

    [HttpPost("bookings")]
    public async Task<IActionResult> GetBookings(BookingFilterRequest request)
    {
        var bookings = await _adminService.GetBookingsAsync(request);
        return Ok(bookings);
    }
    [HttpGet("bookings")]
    public async Task<IActionResult> GetAllBookings(
        [FromQuery] BookingQuery query)
    {
        var result = await _bookingService.GetAllBookingsAsync(query);
        return Ok(result);
    }
}
