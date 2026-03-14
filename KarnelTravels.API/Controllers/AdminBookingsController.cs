using System.Security.Claims;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Services;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminBookingsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;
    private readonly IEmailService _emailService;

    public AdminBookingsController(KarnelTravelsDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    /// <summary>
    /// Lấy danh sách đơn đặt có phân trang, tìm kiếm và lọc (F191, F194, F195, F196)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminBookingDto>>>> GetBookings(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] BookingType? type = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Hotel)
            .Include(b => b.HotelRoom)
            .Include(b => b.TourPackage)
            .Include(b => b.Resort)
            .Include(b => b.ResortRoom)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => 
                b.ContactName.Contains(search) || 
                b.ContactEmail.Contains(search) ||
                b.BookingCode.Contains(search) ||
                (b.User != null && b.User.FullName.Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(b => b.Type == type.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(b => b.ServiceDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(b => b.ServiceDate <= endDate.Value);
        }

        var totalCount = await query.CountAsync();

        var bookings = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var bookingDtos = bookings.Select(b => new AdminBookingDto
        {
            BookingId = b.Id,
            BookingCode = b.BookingCode,
            Type = b.Type,
            Status = b.Status,
            ContactName = b.ContactName,
            ContactEmail = b.ContactEmail,
            ContactPhone = b.ContactPhone,
            ServiceName = GetServiceName(b),
            ServiceDate = b.ServiceDate,
            EndDate = b.EndDate,
            Quantity = b.Quantity,
            TotalAmount = b.TotalAmount,
            DiscountAmount = b.DiscountAmount,
            FinalAmount = b.FinalAmount,
            PaymentStatus = b.PaymentStatus,
            PaymentMethod = b.PaymentMethod,
            Notes = b.Notes,
            CreatedAt = b.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<AdminBookingDto>
        {
            Items = bookingDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(new ApiResponse<PagedResult<AdminBookingDto>>
        {
            Success = true,
            Data = pagedResult
        });
    }

    /// <summary>
    /// Lấy chi tiết một đơn đặt (F192)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<AdminBookingDetailDto>>> GetBooking(Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Hotel)
            .Include(b => b.HotelRoom)
            .Include(b => b.TourPackage)
            .Include(b => b.Resort)
            .Include(b => b.ResortRoom)
            .Include(b => b.Transport)
            .Include(b => b.Restaurant)
            .Include(b => b.Promotion)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound(new ApiResponse<AdminBookingDetailDto>
            {
                Success = false,
                Message = "Không tìm thấy đơn đặt"
            });
        }

        var detail = new AdminBookingDetailDto
        {
            BookingId = booking.Id,
            BookingCode = booking.BookingCode,
            Type = booking.Type,
            Status = booking.Status,
            ContactName = booking.ContactName,
            ContactEmail = booking.ContactEmail,
            ContactPhone = booking.ContactPhone,
            ServiceDate = booking.ServiceDate,
            EndDate = booking.EndDate,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            DiscountAmount = booking.DiscountAmount,
            FinalAmount = booking.FinalAmount,
            PaymentStatus = booking.PaymentStatus,
            PaymentMethod = booking.PaymentMethod,
            PaidAt = booking.PaidAt,
            Notes = booking.Notes,
            CancellationReason = booking.CancellationReason,
            CancelledAt = booking.CancelledAt,
            CreatedAt = booking.CreatedAt,
            
            // Customer info
            CustomerName = booking.User?.FullName,
            CustomerEmail = booking.User?.Email,
            
            // Service details
            HotelName = booking.Hotel?.Name,
            HotelAddress = booking.Hotel?.Address,
            HotelCity = booking.Hotel?.City,
            RoomType = booking.HotelRoom?.RoomType,
            ResortName = booking.Resort?.Name,
            ResortRoomType = booking.ResortRoom?.RoomType,
            TourName = booking.TourPackage?.Name,
            TransportInfo = booking.Transport != null ? $"{booking.Transport.Provider} - {booking.Transport.FromCity} → {booking.Transport.ToCity}" : null,
            RestaurantName = booking.Restaurant?.Name,
            
            // Promotion
            PromotionCode = booking.Promotion?.Code,
            
            // Check-in/out times
            CheckInTime = booking.Hotel?.CheckInTime,
            CheckOutTime = booking.Hotel?.CheckOutTime
        };

        return Ok(new ApiResponse<AdminBookingDetailDto>
        {
            Success = true,
            Data = detail
        });
    }

    /// <summary>
    /// Cập nhật trạng thái đơn đặt (F193, F197)
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<AdminBookingDto>>> UpdateBookingStatus(
        Guid id, 
        [FromBody] UpdateBookingStatusRequest request)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
        {
            return NotFound(new ApiResponse<AdminBookingDto>
            {
                Success = false,
                Message = "Không tìm thấy đơn đặt"
            });
        }

        // Validate status transition
        var (isValid, errorMessage) = ValidateStatusTransition(booking.Status, request.Status);
        if (!isValid)
        {
            return BadRequest(new ApiResponse<AdminBookingDto>
            {
                Success = false,
                Message = errorMessage
            });
        }

        var oldStatus = booking.Status;
        booking.Status = request.Status;
        
        // Handle specific status changes
        if (request.Status == BookingStatus.Cancelled)
        {
            booking.CancellationReason = request.CancellationReason;
            booking.CancelledAt = DateTime.UtcNow;
        }

        booking.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var bookingDto = new AdminBookingDto
        {
            BookingId = booking.Id,
            BookingCode = booking.BookingCode,
            Type = booking.Type,
            Status = booking.Status,
            ContactName = booking.ContactName,
            ContactEmail = booking.ContactEmail,
            ContactPhone = booking.ContactPhone,
            ServiceName = GetServiceName(booking),
            ServiceDate = booking.ServiceDate,
            EndDate = booking.EndDate,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            DiscountAmount = booking.DiscountAmount,
            FinalAmount = booking.FinalAmount,
            PaymentStatus = booking.PaymentStatus,
            PaymentMethod = booking.PaymentMethod,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt
        };

        return Ok(new ApiResponse<AdminBookingDto>
        {
            Success = true,
            Data = bookingDto,
            Message = $"Cập nhật trạng thái thành công: {GetStatusText(request.Status)}"
        });
    }

    /// <summary>
    /// Gửi email xác nhận đơn đặt (F198)
    /// </summary>
    [HttpPost("{id}/send-confirmation")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> SendConfirmationEmail(Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.HotelRoom)
            .Include(b => b.Resort)
            .Include(b => b.ResortRoom)
            .Include(b => b.TourPackage)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy đơn đặt"
            });
        }

        var serviceName = GetServiceName(booking);
        var checkIn = booking.ServiceDate ?? DateTime.Now;
        var checkOut = booking.EndDate ?? checkIn.AddDays(1);

        var success = await _emailService.SendBookingConfirmationAsync(
            booking.ContactEmail,
            booking.ContactName,
            booking.BookingCode,
            serviceName,
            checkIn,
            checkOut,
            booking.FinalAmount
        );

        if (success)
        {
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Gửi email xác nhận thành công"
            });
        }
        else
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Gửi email thất bại. Vui lòng kiểm tra cấu hình email."
            });
        }
    }

    /// <summary>
    /// Xuất danh sách đơn đặt ra file (F199)
    /// </summary>
    [HttpGet("export")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> ExportBookings(
        [FromQuery] string? search = null,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] BookingType? type = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string format = "csv")
    {
        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Hotel)
            .Include(b => b.TourPackage)
            .AsQueryable();

        // Apply filters (same as GetBookings)
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => 
                b.ContactName.Contains(search) || 
                b.ContactEmail.Contains(search) ||
                b.BookingCode.Contains(search));
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(b => b.Type == type.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(b => b.ServiceDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(b => b.ServiceDate <= endDate.Value);
        }

        var bookings = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();

        // Generate CSV content
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Mã đặt,Loại,Khách hàng,Email,SĐT,Ngày đặt,Ngày sử dụng,Ngày kết thúc,Tổng tiền,Giảm giá,Thanh toán,Trạng thái,Phương thức TT");

        foreach (var b in bookings)
        {
            csv.AppendLine($"\"{b.BookingCode}\",\"{GetTypeText(b.Type)}\",\"{b.ContactName}\",\"{b.ContactEmail}\",\"{b.ContactPhone}\",\"{b.CreatedAt:dd/MM/yyyy}\",\"{b.ServiceDate:dd/MM/yyyy}\",\"{b.EndDate:dd/MM/yyyy}\",{b.TotalAmount},{b.DiscountAmount},{b.FinalAmount},\"{GetPaymentStatusText(b.PaymentStatus)}\",\"{b.PaymentMethod}\",\"{GetStatusText(b.Status)}\"");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"bookings_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    /// <summary>
    /// Lấy thống kê doanh thu (F200)
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<BookingStatisticsDto>>> GetStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] BookingType? type = null)
    {
        var query = _context.Bookings.Where(b => b.Status == BookingStatus.Completed);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.ServiceDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(b => b.ServiceDate <= endDate.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(b => b.Type == type.Value);
        }

        var completedBookings = await query.ToListAsync();

        var statistics = new BookingStatisticsDto
        {
            TotalBookings = completedBookings.Count,
            TotalRevenue = completedBookings.Sum(b => b.FinalAmount),
            AverageOrderValue = completedBookings.Any() ? completedBookings.Average(b => b.FinalAmount) : 0,
            PendingCount = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
            ConfirmedCount = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Confirmed),
            CompletedCount = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Completed),
            CancelledCount = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Cancelled),
            StartDate = startDate,
            EndDate = endDate
        };

        return Ok(new ApiResponse<BookingStatisticsDto>
        {
            Success = true,
            Data = statistics
        });
    }

    // Helper methods
    private static string GetServiceName(Booking b)
    {
        return b.Type switch
        {
            BookingType.Hotel => b.Hotel?.Name ?? "Khách sạn",
            BookingType.Tour => b.TourPackage?.Name ?? "Tour du lịch",
            BookingType.Resort => b.Resort?.Name ?? "Resort",
            BookingType.Transport => "Phương tiện",
            BookingType.Restaurant => b.Restaurant?.Name ?? "Nhà hàng",
            _ => "Dịch vụ"
        };
    }

    private static (bool isValid, string? errorMessage) ValidateStatusTransition(BookingStatus current, BookingStatus newStatus)
    {
        // Cannot cancel completed bookings
        if (current == BookingStatus.Completed && newStatus == BookingStatus.Cancelled)
        {
            return (false, "Không thể hủy đơn đã hoàn thành");
        }

        // Cannot change from cancelled to other statuses (except refund)
        if (current == BookingStatus.Cancelled && newStatus != BookingStatus.Refunded)
        {
            return (false, "Không thể thay đổi trạng thái của đơn đã hủy");
        }

        // Refunded can only be set once
        if (current == BookingStatus.Refunded)
        {
            return (false, "Đơn đã được hoàn tiền");
        }

        return (true, null);
    }

    private static string GetStatusText(BookingStatus status) => status switch
    {
        BookingStatus.Pending => "Chờ xác nhận",
        BookingStatus.Confirmed => "Đã xác nhận",
        BookingStatus.Completed => "Hoàn thành",
        BookingStatus.Cancelled => "Đã hủy",
        BookingStatus.Refunded => "Đã hoàn tiền",
        _ => status.ToString()
    };

    private static string GetTypeText(BookingType type) => type switch
    {
        BookingType.Hotel => "Khách sạn",
        BookingType.Tour => "Tour",
        BookingType.Resort => "Resort",
        BookingType.Transport => "Phương tiện",
        BookingType.Restaurant => "Nhà hàng",
        _ => type.ToString()
    };

    private static string GetPaymentStatusText(PaymentStatus status) => status switch
    {
        PaymentStatus.Pending => "Chờ thanh toán",
        PaymentStatus.Paid => "Đã thanh toán",
        PaymentStatus.Failed => "Thất bại",
        PaymentStatus.Refunded => "Đã hoàn tiền",
        _ => status.ToString()
    };
}

// ==================== DTOs ====================

public class AdminBookingDto
{
    public Guid BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public BookingType Type { get; set; }
    public BookingStatus Status { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminBookingDetailDto : AdminBookingDto
{
    public DateTime? PaidAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Customer info
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    
    // Service details
    public string? HotelName { get; set; }
    public string? HotelAddress { get; set; }
    public string? HotelCity { get; set; }
    public string? RoomType { get; set; }
    public string? ResortName { get; set; }
    public string? ResortRoomType { get; set; }
    public string? TourName { get; set; }
    public string? TransportInfo { get; set; }
    public string? RestaurantName { get; set; }
    
    // Promotion
    public string? PromotionCode { get; set; }
    
    // Check-in/out
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
}

public class UpdateBookingStatusRequest
{
    public BookingStatus Status { get; set; }
    public string? CancellationReason { get; set; }
}

public class BookingStatisticsDto
{
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
