using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarnelTravels.API.Data;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminDashboardController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public AdminDashboardController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    // F161-F166: Dashboard Summary Statistics
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetDashboardSummary()
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        // F161: Total Destinations
        var totalDestinations = await _context.TouristSpots
            .Where(s => !s.IsDeleted)
            .CountAsync();

        // F162: Total Hotels
        var totalHotels = await _context.Hotels
            .Where(h => !h.IsDeleted)
            .CountAsync();

        // F163: Total Restaurants
        var totalRestaurants = await _context.Restaurants
            .Where(r => !r.IsDeleted)
            .CountAsync();

        // F164: Total Resorts
        var totalResorts = await _context.Resorts
            .Where(r => !r.IsDeleted)
            .CountAsync();

        // F165: Today's Bookings
        var todayBookingsCount = await _context.Bookings
            .Where(b => !b.IsDeleted && b.CreatedAt.Date == today)
            .CountAsync();

        // F166: Monthly Revenue
        var monthlyRevenue = await _context.Bookings
            .Where(b => !b.IsDeleted 
                && b.PaymentStatus == PaymentStatus.Paid
                && b.CreatedAt >= startOfMonth 
                && b.CreatedAt <= endOfMonth)
            .SumAsync(b => b.FinalAmount);

        // Additional stats
        var totalUsers = await _context.Users
            .Where(u => !u.IsDeleted)
            .CountAsync();

        var totalTours = await _context.TourPackages
            .Where(t => !t.IsDeleted)
            .CountAsync();

        var pendingBookings = await _context.Bookings
            .Where(b => !b.IsDeleted && b.Status == BookingStatus.Pending)
            .CountAsync();

        var unreadContacts = await _context.Contacts
            .Where(c => !c.IsDeleted && c.Status == ContactStatus.Unread)
            .CountAsync();

        var summary = new DashboardSummaryDto
        {
            TotalDestinations = totalDestinations,
            TotalHotels = totalHotels,
            TotalRestaurants = totalRestaurants,
            TotalResorts = totalResorts,
            TodayBookingsCount = todayBookingsCount,
            MonthlyRevenue = monthlyRevenue,
            TotalUsers = totalUsers,
            TotalTours = totalTours,
            PendingBookings = pendingBookings,
            UnreadContacts = unreadContacts
        };

        return Ok(new ApiResponse<DashboardSummaryDto>
        {
            Success = true,
            Message = "Dashboard summary retrieved successfully",
            Data = summary
        });
    }

    // F168: Revenue Chart Data (12 months)
    [HttpGet("charts")]
    public async Task<ActionResult<ApiResponse<RevenueChartDto>>> GetRevenueChart()
    {
        var today = DateTime.UtcNow.Date;
        var twelveMonthsAgo = today.AddMonths(-11);
        
        // Get the first day of the month 12 months ago
        var startDate = new DateTime(twelveMonthsAgo.Year, twelveMonthsAgo.Month, 1);

        var bookings = await _context.Bookings
            .Where(b => !b.IsDeleted 
                && b.PaymentStatus == PaymentStatus.Paid
                && b.CreatedAt >= startDate)
            .ToListAsync();

        var monthlyRevenues = new List<MonthlyRevenue>();
        var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
                                 "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        for (int i = 0; i < 12; i++)
        {
            var currentMonth = startDate.AddMonths(i);
            var monthBookings = bookings
                .Where(b => b.CreatedAt.Year == currentMonth.Year 
                    && b.CreatedAt.Month == currentMonth.Month)
                .ToList();

            monthlyRevenues.Add(new MonthlyRevenue
            {
                Month = currentMonth.Month,
                MonthName = monthNames[currentMonth.Month - 1],
                Revenue = monthBookings.Sum(b => b.FinalAmount),
                BookingCount = monthBookings.Count
            });
        }

        var totalYearRevenue = monthlyRevenues.Sum(m => m.Revenue);

        var chartData = new RevenueChartDto
        {
            MonthlyRevenues = monthlyRevenues,
            TotalYearRevenue = totalYearRevenue
        };

        return Ok(new ApiResponse<RevenueChartDto>
        {
            Success = true,
            Message = "Revenue chart data retrieved successfully",
            Data = chartData
        });
    }

    // F167: Recent Bookings (5 latest)
    [HttpGet("recent-bookings")]
    public async Task<ActionResult<ApiResponse<List<RecentBookingDto>>>> GetRecentBookings()
    {
        var recentBookings = await _context.Bookings
            .Where(b => !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Select(b => new RecentBookingDto
            {
                BookingId = b.Id,
                BookingCode = b.BookingCode,
                CustomerName = b.ContactName,
                CustomerEmail = b.ContactEmail,
                ServiceName = b.TourPackage != null ? b.TourPackage.Name :
                            b.Hotel != null ? b.Hotel.Name :
                            b.Resort != null ? b.Resort.Name :
                            b.Transport != null ? b.Transport.Provider :
                            b.Restaurant != null ? b.Restaurant.Name : "N/A",
                Type = b.Type.ToString(),
                Status = b.Status.ToString(),
                TotalAmount = b.FinalAmount,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<RecentBookingDto>>
        {
            Success = true,
            Message = "Recent bookings retrieved successfully",
            Data = recentBookings
        });
    }

    // F169: Unread Contacts
    [HttpGet("unread-contacts")]
    public async Task<ActionResult<ApiResponse<List<UnreadContactDto>>>> GetUnreadContacts()
    {
        var unreadContacts = await _context.Contacts
            .Where(c => !c.IsDeleted && c.Status == ContactStatus.Unread)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new UnreadContactDto
            {
                ContactId = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                ServiceType = c.ServiceType,
                Message = c.Message,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<UnreadContactDto>>
        {
            Success = true,
            Message = "Unread contacts retrieved successfully",
            Data = unreadContacts
        });
    }
}
