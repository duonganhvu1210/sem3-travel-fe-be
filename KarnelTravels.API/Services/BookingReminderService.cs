using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Services;

/// <summary>
/// Background service for sending booking reminders (F356)
/// Simulates sending email/SMS reminders 1 day before service
/// </summary>
public class BookingReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingReminderService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

    public BookingReminderService(IServiceProvider serviceProvider, ILogger<BookingReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Booking Reminder Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndSendRemindersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Booking Reminder Service");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckAndSendRemindersAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<KarnelTravelsDbContext>();

        // Find bookings that need reminders (1 day before service)
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var tomorrowEnd = tomorrow.AddDays(1);

        var bookingsToRemind = await context.Bookings
            .Include(b => b.User)
            .Where(b => !b.IsDeleted
                && b.Status == BookingStatus.Confirmed
                && b.ServiceDate >= tomorrow
                && b.ServiceDate < tomorrowEnd
                && b.PaymentStatus == PaymentStatus.Paid)
            .ToListAsync();

        foreach (var booking in bookingsToRemind)
        {
            // Check if reminder already sent (you would track this in a separate table in production)
            // For now, we'll log the reminder
            _logger.LogInformation(
                "[REMINDER] Sending reminder for booking {BookingCode}. Service date: {ServiceDate}. Contact: {Email}",
                booking.BookingCode,
                booking.ServiceDate?.ToString("dd/MM/yyyy") ?? "N/A",
                booking.ContactEmail);

            // In production, you would:
            // 1. Check if reminder was already sent (track in ReminderLogs table)
            // 2. Send email via EmailService
            // 3. Send SMS if phone available
            // 4. Log to ReminderLogs table
            
            // Simulate sending
            await SimulateSendReminderAsync(booking);
        }

        if (bookingsToRemind.Any())
        {
            _logger.LogInformation("Sent {Count} booking reminders", bookingsToRemind.Count);
        }
    }

    private Task SimulateSendReminderAsync(Booking booking)
    {
        // In production, inject IEmailService and ISmsService
        var reminderMessage = $@"
Kính gửi {booking.ContactName},

Đơn hàng {booking.BookingCode} của quý khách sẽ được sử dụng vào ngày mai.

Thông tin đơn hàng:
- Mã đơn: {booking.BookingCode}
- Ngày sử dụng: {booking.ServiceDate:dd/MM/yyyy}
- Dịch vụ: {booking.Type}

Cảm ơn quý khách đã sử dụng dịch vụ của Karnel Travels!

---
Email tự động. Vui lòng không trả lời email này.
";

        _logger.LogInformation("Reminder content for {Email}: {Message}", 
            booking.ContactEmail, 
            reminderMessage);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Extension methods for registering background services
/// </summary>
public static class BookingReminderServiceExtensions
{
    public static IServiceCollection AddBookingReminderService(this IServiceCollection services)
    {
        services.AddHostedService<BookingReminderService>();
        return services;
    }
}
