using BookingSystem.Data;
using BookingSystem.Enums;
using BookingSystem.Exceptions;
using BookingSystem.Models;
using BookingSystem.Models.Auth;
using BookingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Services;

public class AuthService : IAuthService
{
    private readonly BookingDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        BookingDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (exists)
            throw new BadRequestException("User already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Customer
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new UnauthorizedException("Invalid credentials");

        var validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!validPassword)
            throw new UnauthorizedException("Invalid credentials");

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwt = _configuration.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(jwt["ExpireMinutes"]!)
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
