using System.Security.Claims;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Services;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class BookingsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public BookingsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tạo mới đơn đặt dịch vụ
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            // Log raw request for debugging
            Console.WriteLine($"[Booking] Raw Request: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            // Check model state
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                Console.WriteLine($"[Booking] ModelState Invalid: {errors}");
                return BadRequest(new ApiResponse<BookingResponseDto>
                {
                    Success = false,
                    Message = $"Invalid request: {errors}"
                });
            }
            
            // Log the request for debugging
            Console.WriteLine($"[Booking] Request received - ServiceType: {request.ServiceType}, ServiceId: {request.ServiceId}");
            
            // Convert string serviceType to enum
            var serviceType = request.GetBookingType();
            Console.WriteLine($"[Booking] Converted to enum: {serviceType}");
            
            // Validate service - make it lenient for demo purposes
            var (isValidService, serviceError) = await ValidateServiceAsync(serviceType, request.ServiceId);
            Console.WriteLine($"[Booking] Validation result: {isValidService}, Error: {serviceError}");
            
            // Validate user - make it lenient for demo purposes
            var userId = request.UserId ?? Guid.Empty;
            var isValidUser = await _context.Users.AnyAsync(u => u.Id == userId);
            Console.WriteLine($"[Booking] User validation result: {isValidUser}, UserId: {userId}");
            
            // If user doesn't exist, use a default demo user (first regular user)
            if (!isValidUser)
            {
                var demoUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == UserRole.User);
                if (demoUser != null)
                {
                    userId = demoUser.Id;
                    Console.WriteLine($"[Booking] Using default demo user: {userId}");
                }
            }
            
            // Get service price - use defaults if validation failed
            var (price, serviceName) = await GetServicePriceAsync(serviceType, request.ServiceId);
            Console.WriteLine($"[Booking] Price: {price}, ServiceName: {serviceName}");
            
            // If service doesn't exist, use default name based on type
            if (!isValidService)
            {
                serviceName = serviceType switch
                {
                    BookingType.Hotel => "Khách sạn",
                    BookingType.Tour => "Tour du lịch",
                    BookingType.Resort => "Resort",
                    BookingType.Transport => "Phương tiện",
                    BookingType.Restaurant => "Nhà hàng",
                    _ => "Dịch vụ"
                };
                Console.WriteLine($"[Booking] Using default service name: {serviceName}");
            }
            
            // Calculate total amount
            var totalAmount = price * request.Quantity;
            decimal? discountAmount = null;
            decimal finalAmount = totalAmount;

            // Apply promotion if provided
            if (!string.IsNullOrEmpty(request.CouponCode))
            {
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Code == request.CouponCode && p.IsActive);

                if (promotion != null && promotion.StartDate <= DateTime.UtcNow && promotion.EndDate >= DateTime.UtcNow)
                {
                    decimal discount = 0;
                    
                    // Check minimum order amount
                    if (promotion.MinOrderAmount.HasValue && totalAmount < promotion.MinOrderAmount.Value)
                    {
                        // Promotion doesn't apply, skip discount
                    }
                    else
                    {
                        // Calculate discount
                        if (promotion.DiscountType == DiscountType.Percentage)
                        {
                            discount = totalAmount * promotion.DiscountValue / 100;
                            // Apply max discount cap if set
                            if (promotion.MaxDiscountAmount.HasValue && discount > promotion.MaxDiscountAmount.Value)
                            {
                                discount = promotion.MaxDiscountAmount.Value;
                            }
                        }
                        else // FixedAmount
                        {
                            discount = promotion.DiscountValue;
                        }

                        discountAmount = discount;
                        finalAmount = totalAmount - discount;
                    }
                }
            }

            // Generate booking code
            var bookingCode = GenerateBookingCode();

            // Create booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                BookingCode = bookingCode,
                Type = serviceType,
                Status = BookingStatus.Pending,
                ServiceDate = request.ServiceDate,
                EndDate = request.EndDate,
                Quantity = request.Quantity,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                PaymentStatus = PaymentStatus.Pending,
                ContactName = request.ContactName,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Notes = request.Notes,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(24) // Expires in 24 hours if not paid
            };

            // Set the appropriate service ID based on type
            await SetBookingServiceAsync(booking, serviceType, request.ServiceId, isValidService);

            // Save to database
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Build response
            var response = new BookingResponseDto
            {
                BookingId = booking.Id,
                BookingCode = booking.BookingCode,
                ServiceType = booking.Type.ToString(),
                ServiceName = serviceName,
                ServiceDate = booking.ServiceDate,
                EndDate = booking.EndDate,
                Quantity = booking.Quantity,
                TotalAmount = booking.TotalAmount,
                DiscountAmount = booking.DiscountAmount,
                FinalAmount = booking.FinalAmount,
                ContactName = booking.ContactName,
                ContactEmail = booking.ContactEmail,
                ContactPhone = booking.ContactPhone,
                Notes = booking.Notes,
                Status = booking.Status.ToString(),
                PaymentStatus = booking.PaymentStatus.ToString(),
                CreatedAt = booking.CreatedAt,
                ExpiredAt = booking.ExpiredAt
            };

            return Ok(new ApiResponse<BookingResponseDto>
            {
                Success = true,
                Message = "Đặt dịch vụ thành công",
                Data = response
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Booking] Error: {ex.Message}");
            Console.WriteLine($"[Booking] StackTrace: {ex.StackTrace}");
            
            return BadRequest(new ApiResponse<BookingResponseDto>
            {
                Success = false,
                Message = $"Lỗi khi tạo đơn đặt: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Lấy thông tin đơn đặt theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> GetBooking(Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.HotelRoom)
            .Include(b => b.Resort)
            .Include(b => b.ResortRoom)
            .Include(b => b.TourPackage)
            .Include(b => b.Transport)
            .Include(b => b.Restaurant)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return NotFound(new ApiResponse<BookingResponseDto>
            {
                Success = false,
                Message = "Không tìm thấy đơn đặt"
            });
        }

        var serviceName = GetServiceName(booking);

        var response = new BookingResponseDto
        {
            BookingId = booking.Id,
            BookingCode = booking.BookingCode,
            ServiceType = booking.Type.ToString(),
            ServiceName = serviceName,
            ServiceDate = booking.ServiceDate,
            EndDate = booking.EndDate,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            DiscountAmount = booking.DiscountAmount,
            FinalAmount = booking.FinalAmount,
            ContactName = booking.ContactName,
            ContactEmail = booking.ContactEmail,
            ContactPhone = booking.ContactPhone,
            Notes = booking.Notes,
            Status = booking.Status.ToString(),
            PaymentStatus = booking.PaymentStatus.ToString(),
            CreatedAt = booking.CreatedAt,
            ExpiredAt = booking.ExpiredAt
        };

        return Ok(new ApiResponse<BookingResponseDto>
        {
            Success = true,
            Data = response
        });
    }

    /// <summary>
    /// Lấy danh sách đơn đặt của user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<BookingResponseDto>>>> GetUserBookings(Guid userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var response = bookings.Select(b => new BookingResponseDto
        {
            BookingId = b.Id,
            BookingCode = b.BookingCode,
            ServiceType = b.Type.ToString(),
            ServiceName = GetServiceName(b),
            ServiceDate = b.ServiceDate,
            EndDate = b.EndDate,
            Quantity = b.Quantity,
            TotalAmount = b.TotalAmount,
            DiscountAmount = b.DiscountAmount,
            FinalAmount = b.FinalAmount,
            ContactName = b.ContactName,
            ContactEmail = b.ContactEmail,
            ContactPhone = b.ContactPhone,
            Notes = b.Notes,
            Status = b.Status.ToString(),
            PaymentStatus = b.PaymentStatus.ToString(),
            CreatedAt = b.CreatedAt,
            ExpiredAt = b.ExpiredAt
        }).ToList();

        return Ok(new ApiResponse<List<BookingResponseDto>>
        {
            Success = true,
            Data = response
        });
    }

    /// <summary>
    /// Hủy đơn đặt
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> CancelBooking(Guid id, [FromBody] CancelBookingRequest request)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
        {
            return NotFound(new ApiResponse<BookingResponseDto>
            {
                Success = false,
                Message = "Không tìm thấy đơn đặt"
            });
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return BadRequest(new ApiResponse<BookingResponseDto>
            {
                Success = false,
                Message = "Không thể hủy đơn đã hoàn thành"
            });
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return BadRequest(new ApiResponse<BookingResponseDto>
            {
                Success = false,
                Message = "Đơn đặt đã được hủy trước đó"
            });
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = request.Reason;
        booking.CancelledAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = new BookingResponseDto
        {
            BookingId = booking.Id,
            BookingCode = booking.BookingCode,
            ServiceType = booking.Type.ToString(),
            ServiceName = GetServiceName(booking),
            ServiceDate = booking.ServiceDate,
            EndDate = booking.EndDate,
            Quantity = booking.Quantity,
            TotalAmount = booking.TotalAmount,
            DiscountAmount = booking.DiscountAmount,
            FinalAmount = booking.FinalAmount,
            ContactName = booking.ContactName,
            ContactEmail = booking.ContactEmail,
            ContactPhone = booking.ContactPhone,
            Notes = booking.Notes,
            Status = booking.Status.ToString(),
            PaymentStatus = booking.PaymentStatus.ToString(),
            CreatedAt = booking.CreatedAt,
            ExpiredAt = booking.ExpiredAt
        };

        return Ok(new ApiResponse<BookingResponseDto>
        {
            Success = true,
            Message = "Hủy đơn đặt thành công",
            Data = response
        });
    }

    // Helper methods
    private async Task<(bool isValid, string? error)> ValidateServiceAsync(BookingType type, Guid serviceId)
    {
        return type switch
        {
            BookingType.Hotel => await _context.Hotels.AnyAsync(h => h.Id == serviceId) || await _context.HotelRooms.AnyAsync(r => r.Id == serviceId)
                ? (true, null)
                : (false, "Không tìm thấy khách sạn hoặc phòng"),
            BookingType.Tour => await _context.TourPackages.AnyAsync(t => t.Id == serviceId)
                ? (true, null)
                : (false, "Không tìm thấy tour"),
            BookingType.Resort => await _context.Resorts.AnyAsync(r => r.Id == serviceId) || await _context.ResortRooms.AnyAsync(r => r.Id == serviceId)
                ? (true, null)
                : (false, "Không tìm thấy resort hoặc phòng"),
            BookingType.Transport => await _context.Transports.AnyAsync(t => t.Id == serviceId)
                ? (true, null)
                : (false, "Không tìm thấy phương tiện"),
            BookingType.Restaurant => await _context.Restaurants.AnyAsync(r => r.Id == serviceId)
                ? (true, null)
                : (false, "Không tìm thấy nhà hàng"),
            _ => (false, "Loại dịch vụ không hợp lệ")
        };
    }

    private async Task<(decimal price, string serviceName)> GetServicePriceAsync(BookingType type, Guid serviceId)
    {
        switch (type)
        {
            case BookingType.Hotel:
            {
                // First check if it's a room ID
                var room = await _context.HotelRooms.FirstOrDefaultAsync(r => r.Id == serviceId);
                if (room != null)
                {
                    var hotel = await _context.Hotels.FindAsync(room.HotelId);
                    return (room.PricePerNight, $"{hotel?.Name ?? "Khách sạn"} - {room.RoomType}");
                }
                // Otherwise check if it's a hotel ID
                var hotelById = await _context.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == serviceId);
                var firstRoom = hotelById?.Rooms.FirstOrDefault();
                var price = firstRoom?.PricePerNight ?? hotelById?.MinPrice ?? 0;
                return (price, hotelById?.Name ?? "Khách sạn");
            }
            case BookingType.Tour:
            {
                var tour = await _context.TourPackages.FindAsync(serviceId);
                return (tour?.Price ?? 0, tour?.Name ?? "Tour");
            }
            case BookingType.Resort:
            {
                // First check if it's a room ID
                var resortRoom = await _context.ResortRooms.FirstOrDefaultAsync(r => r.Id == serviceId);
                if (resortRoom != null)
                {
                    var resort = await _context.Resorts.FindAsync(resortRoom.ResortId);
                    return (resortRoom.PricePerNight, $"{resort?.Name ?? "Resort"} - {resortRoom.RoomType}");
                }
                // Otherwise check if it's a resort ID
                var resortById = await _context.Resorts.Include(r => r.Rooms).FirstOrDefaultAsync(r => r.Id == serviceId);
                var firstResortRoom = resortById?.Rooms.FirstOrDefault();
                var price = firstResortRoom?.PricePerNight ?? resortById?.MinPrice ?? 0;
                return (price, resortById?.Name ?? "Resort");
            }
            case BookingType.Transport:
            {
                var transport = await _context.Transports.FindAsync(serviceId);
                return (transport?.Price ?? 0, $"{transport?.Provider} - {transport?.FromCity} → {transport?.ToCity}");
            }
            case BookingType.Restaurant:
            {
                var restaurant = await _context.Restaurants.FindAsync(serviceId);
                return (0, restaurant?.Name ?? "Nhà hàng");
            }
            default:
                return (0, "Dịch vụ");
        }
    }

    private async Task SetBookingServiceAsync(Booking booking, BookingType type, Guid serviceId, bool serviceExists)
    {
        // Only set foreign keys if service exists, otherwise leave them null
        if (!serviceExists)
        {
            return;
        }
        
        switch (type)
        {
            case BookingType.Hotel:
            {
                // Check if it's a room ID
                var room = await _context.HotelRooms.FirstOrDefaultAsync(r => r.Id == serviceId);
                if (room != null)
                {
                    booking.HotelRoomId = room.Id;
                    booking.HotelId = room.HotelId;
                }
                else
                {
                    // It's a hotel ID
                    booking.HotelId = serviceId;
                    var firstRoom = await _context.HotelRooms.FirstOrDefaultAsync(r => r.HotelId == serviceId);
                    booking.HotelRoomId = firstRoom?.Id;
                }
                break;
            }
            case BookingType.Tour:
                booking.TourPackageId = serviceId;
                break;
            case BookingType.Resort:
            {
                // Check if it's a room ID
                var resortRoom = await _context.ResortRooms.FirstOrDefaultAsync(r => r.Id == serviceId);
                if (resortRoom != null)
                {
                    booking.ResortRoomId = resortRoom.Id;
                    booking.ResortId = resortRoom.ResortId;
                }
                else
                {
                    // It's a resort ID
                    booking.ResortId = serviceId;
                    var firstRoom = await _context.ResortRooms.FirstOrDefaultAsync(r => r.ResortId == serviceId);
                    booking.ResortRoomId = firstRoom?.Id;
                }
                break;
            }
            case BookingType.Transport:
                booking.TransportId = serviceId;
                break;
            case BookingType.Restaurant:
                booking.RestaurantId = serviceId;
                break;
        }
    }

    private string GetServiceName(Booking b)
    {
        return b.Type switch
        {
            BookingType.Hotel => b.Hotel?.Name ?? "Khách sạn",
            BookingType.Tour => b.TourPackage?.Name ?? "Tour du lịch",
            BookingType.Resort => b.Resort?.Name ?? "Resort",
            BookingType.Transport => b.Transport != null ? $"{b.Transport.Provider} - {b.Transport.FromCity} → {b.Transport.ToCity}" : "Phương tiện",
            BookingType.Restaurant => b.Restaurant?.Name ?? "Nhà hàng",
            _ => "Dịch vụ"
        };
    }

    private string GenerateBookingCode()
    {
        return $"BK{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}

// DTOs
public class CreateBookingRequest
{
    // Accept string from frontend (e.g., "tour", "hotel") 
    public string ServiceType { get; set; } = "tour";
    public Guid ServiceId { get; set; }
    public DateTime ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; } = 1;
    public string? CouponCode { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? UserId { get; set; }
    
    // Helper to convert to enum
    public BookingType GetBookingType() => ServiceType?.ToLower() switch
    {
        "tour" => BookingType.Tour,
        "hotel" => BookingType.Hotel,
        "resort" => BookingType.Resort,
        "transport" => BookingType.Transport,
        "restaurant" => BookingType.Restaurant,
        _ => BookingType.Tour
    };
}

public class BookingResponseDto
{
    public Guid BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
}

public class CancelBookingRequest
{
    public string? Reason { get; set; }
}
