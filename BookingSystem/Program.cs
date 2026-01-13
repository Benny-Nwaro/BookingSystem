
using BookingSystem.Data;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Services.Interfaces;
using BookingSystem.Services;
using BookingSystem.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IWhatsAppSender, WhatsAppSender>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


builder.Services.AddControllers();
builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseSqlite("Data Source=booking.db");
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
   
}



app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
