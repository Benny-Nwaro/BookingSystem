using BookingSystem.Enums;
using BookingSystem.Models.Common;

namespace BookingSystem.Models;

public class BookingQuery : PagedRequest
{
    public BookingStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
