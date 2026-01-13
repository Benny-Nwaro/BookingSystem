using BookingSystem.Models.Auth;

namespace BookingSystem.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
