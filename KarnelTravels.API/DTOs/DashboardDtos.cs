namespace KarnelTravels.API.DTOs;

// Dashboard Summary DTO (F161-F166)
public class DashboardSummaryDto
{
    public int TotalDestinations { get; set; }
    public int TotalHotels { get; set; }
    public int TotalRestaurants { get; set; }
    public int TotalResorts { get; set; }
    public int TodayBookingsCount { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int TotalUsers { get; set; }
    public int TotalTours { get; set; }
    public int PendingBookings { get; set; }
    public int UnreadContacts { get; set; }
}

// Revenue Chart Data DTO (F168)
public class RevenueChartDto
{
    public List<MonthlyRevenue> MonthlyRevenues { get; set; } = new();
    public decimal TotalYearRevenue { get; set; }
}

public class MonthlyRevenue
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int BookingCount { get; set; }
}

// Recent Booking DTO (F167)
public class RecentBookingDto
{
    public Guid BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Contact DTO for Unread (F169)
public class UnreadContactDto
{
    public Guid ContactId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ServiceType { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Notification DTO (F170)
public class BookingNotificationDto
{
    public Guid BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
