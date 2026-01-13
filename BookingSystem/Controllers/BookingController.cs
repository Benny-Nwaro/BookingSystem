using BookingSystem.Models.DTOs;
using BookingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookingSystem.Exceptions;
using BookingSystem.Models;
namespace BookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Availability search (public)
    [HttpPost("availability")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchAvailability(
        [FromBody] AvailabilityRequest request)
    {
        var rooms = await _bookingService.SearchAvailabilityAsync(request);
        return Ok(rooms);
    }

    // Create booking
    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingRequest request)
    {
        var userId = GetUserId();
        await _bookingService.CreateBookingAsync(userId, request);

        return Ok(new
        {
            message = "Booking confirmed successfully."
        });
    }

    [HttpPost("hold")]
    public async Task<IActionResult> CreateHold(
        [FromBody] CreateHoldRequest request)
    {
        var userId = GetUserId();
        var hold = await _bookingService.CreateHoldAsync(userId, request);

        return Ok(new
        {
            hold.Id,
            hold.RoomId,
            hold.CheckInDate,
            hold.CheckOutDate,
            hold.HoldExpiresAt
        });
    }

    [HttpDelete("{bookingId}")]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        var userId = GetUserId();
        await _bookingService.CancelBookingAsync(bookingId, userId);

        return Ok(new
        {
            message = "Booking cancelled successfully."
        });
    }

    // Admin-only cleanup
    [HttpPost("cleanup-holds")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CleanupExpiredHolds()
    {
        await _bookingService.CleanupExpiredHoldsAsync();

        return Ok(new
        {
            message = "Expired holds cleaned up."
        });
    }

    //  My bookings with (pagination + filtering)
    [HttpGet("my")]
    public async Task<IActionResult> MyBookings(
        [FromQuery] BookingQuery query)
    {
        var userId = GetUserId();
        var result = await _bookingService.GetUserBookingsAsync(userId, query);

        return Ok(result);
    }

    // ingle source of truth for UserId
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user identity");

        return userId;
    }
}
