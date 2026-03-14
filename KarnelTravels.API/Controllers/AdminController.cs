using System.Security.Claims;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public AdminController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    // ==================== DASHBOARD STATS ====================

    /// <summary>
    /// Lấy thống kê dashboard - Admin only
    /// </summary>
    [HttpGet("dashboard-stats")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<DashboardStats>>> GetDashboardStats()
    {
        var stats = new DashboardStats
        {
            TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted),
            TotalBookings = await _context.Bookings.CountAsync(b => !b.IsDeleted),
            TotalRevenue = await _context.Bookings
                .Where(b => !b.IsDeleted && b.Status == BookingStatus.Confirmed)
                .SumAsync(b => b.FinalAmount),
            TotalTours = await _context.TourPackages.CountAsync(t => !t.IsDeleted),
            TotalHotels = await _context.Hotels.CountAsync(h => !h.IsDeleted),
            PendingBookings = await _context.Bookings
                .CountAsync(b => !b.IsDeleted && b.Status == BookingStatus.Pending)
        };

        return Ok(new ApiResponse<DashboardStats>
        {
            Success = true,
            Data = stats
        });
    }

    // ==================== USER MANAGEMENT ====================

    /// <summary>
    /// Lấy danh sách tất cả users - Admin only
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
    {
        var users = await _context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new UserDto
            {
                UserId = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                IsLocked = u.IsLocked,
                IsVerified = u.IsEmailVerified,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<UserDto>>
        {
            Success = true,
            Data = users
        });
    }

    /// <summary>
    /// Khóa/Mở khóa user - Admin only
    /// </summary>
    [HttpPut("users/{userId}/toggle-lock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> ToggleUserLock(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "Không tìm thấy người dùng"
            });
        }

        user.IsLocked = !user.IsLocked;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = user.IsLocked ? "Đã khóa người dùng" : "Đã mở khóa người dùng"
        });
    }

    /// <summary>
    /// Thăng cấp user lên Admin - Admin only
    /// </summary>
    [HttpPut("users/{userId}/promote")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> PromoteToAdmin(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "Không tìm thấy người dùng"
            });
        }

        user.Role = UserRole.Admin;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Đã thăng cấp người dùng lên Admin"
        });
    }

    // ==================== BOOKING MANAGEMENT ====================

    /// <summary>
    /// Lấy danh sách bookings - Admin và Manager
    /// </summary>
    [HttpGet("bookings")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<BookingDto>>>> GetAllBookings()
    {
        var bookings = await _context.Bookings
            .Where(b => !b.IsDeleted)
            .Include(b => b.User)
            .Select(b => new BookingDto
            {
                BookingId = b.Id,
                UserEmail = b.User != null ? b.User.Email : "",
                TourName = b.TourPackage != null ? b.TourPackage.Name : "",
                TotalPrice = b.FinalAmount,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<BookingDto>>
        {
            Success = true,
            Data = bookings
        });
    }
}

// ==================== DTOs for Admin ====================

public class UserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingDto
{
    public Guid BookingId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string TourName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DashboardStats
{
    public int TotalUsers { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalTours { get; set; }
    public int TotalHotels { get; set; }
    public int PendingBookings { get; set; }
}
