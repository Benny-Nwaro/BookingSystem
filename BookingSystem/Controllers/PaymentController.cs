using BookingSystem.Models.Payment;
using BookingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Pay(
        [FromBody] CreatePaymentRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue("sub")!);
        var result = await _paymentService.ProcessPaymentAsync(userId, request);
        return Ok(result);
    }
}
