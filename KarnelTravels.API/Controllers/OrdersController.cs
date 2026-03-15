using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using KarnelTravels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;
    private readonly ILogger<OrdersController> _logger;
    private readonly IInvoiceService _invoiceService;

    public OrdersController(KarnelTravelsDbContext context, ILogger<OrdersController> logger, IInvoiceService invoiceService)
    {
        _context = context;
        _logger = logger;
        _invoiceService = invoiceService;
    }

    private Guid ResolveUserId(OrderFilterRequest? req = null, Guid? guidId = null)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(claim) && Guid.TryParse(claim, out var fromClaim))
            return fromClaim;
        if (req?.UserId != null)
            return req.UserId.Value;
        if (guidId != null)
            return guidId.Value;
        return _context.Users.FirstOrDefault()?.Id ?? Guid.Empty;
    }

    private DateTime GetServiceDate(Booking o) => o.ServiceDate ?? DateTime.UtcNow;
    private DateTime GetEndDate(Booking o) => o.EndDate ?? DateTime.UtcNow;

    [HttpGet]
    public async Task<ActionResult<PagedOrderResult>> GetOrders([FromQuery] OrderFilterRequest request)
    {
        try
        {
            var userId = ResolveUserId(request);
            var query = _context.Bookings.Where(b => b.UserId == userId && !b.IsDeleted).AsQueryable();

            if (request.Status != null && request.Status != OrderStatusFilter.All)
            {
                var s = request.Status.Value switch
                {
                    OrderStatusFilter.Pending => BookingStatus.Pending,
                    OrderStatusFilter.Confirmed => BookingStatus.Confirmed,
                    OrderStatusFilter.Completed => BookingStatus.Completed,
                    OrderStatusFilter.Cancelled => BookingStatus.Cancelled,
                    _ => BookingStatus.Pending
                };
                query = query.Where(b => b.Status == s);
            }

            if (request.ServiceType != null && request.ServiceType != ServiceTypeFilter.All)
            {
                var t = request.ServiceType.Value switch
                {
                    ServiceTypeFilter.Tour => BookingType.Tour,
                    ServiceTypeFilter.Hotel => BookingType.Hotel,
                    ServiceTypeFilter.Resort => BookingType.Resort,
                    ServiceTypeFilter.Transport => BookingType.Transport,
                    ServiceTypeFilter.Restaurant => BookingType.Restaurant,
                    _ => BookingType.Tour
                };
                query = query.Where(b => b.Type == t);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
                query = query.Where(b => b.BookingCode.ToLower().Contains(request.SearchQuery.ToLower()));

            if (request.FromDate != null)
                query = query.Where(b => b.CreatedAt >= request.FromDate.Value);
            if (request.ToDate != null)
                query = query.Where(b => b.CreatedAt <= request.ToDate.Value);

            var totalCount = await query.CountAsync();

            query = (request.SortBy?.ToLower() ?? "") switch
            {
                "servicedate" => request.SortDescending ? query.OrderByDescending(b => b.ServiceDate) : query.OrderBy(b => b.ServiceDate),
                "totalprice" => request.SortDescending ? query.OrderByDescending(b => b.FinalAmount) : query.OrderBy(b => b.FinalAmount),
                _ => request.SortDescending ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt)
            };

            var orders = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();

            var dtos = orders.Select(o => new OrderListDto
            {
                OrderId = o.Id,
                OrderCode = o.BookingCode,
                ServiceType = o.Type.ToString(),
                ServiceName = GetServiceName(o),
                Status = o.Status.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                TotalPrice = o.FinalAmount,
                BookingDate = o.CreatedAt,
                ServiceDate = GetServiceDate(o),
                EndDate = o.EndDate,
                Quantity = o.Quantity
            }).ToList();

            return Ok(new PagedOrderResult { Orders = dtos, TotalCount = totalCount, PageNumber = request.PageNumber, PageSize = request.PageSize });
        }
        catch (Exception ex) { _logger.LogError(ex, "Error fetching orders"); return StatusCode(500, new { message = "Error", error = ex.Message }); }
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<OrderStatisticsDto>> GetStatistics([FromQuery] Guid? userId)
    {
        try
        {
            var uid = ResolveUserId(guidId: userId);
            var bookings = await _context.Bookings.Where(b => b.UserId == uid && !b.IsDeleted).ToListAsync();
            return Ok(new OrderStatisticsDto
            {
                TotalOrders = bookings.Count,
                PendingOrders = bookings.Count(b => b.Status == BookingStatus.Pending),
                ConfirmedOrders = bookings.Count(b => b.Status == BookingStatus.Confirmed),
                CompletedOrders = bookings.Count(b => b.Status == BookingStatus.Completed),
                CancelledOrders = bookings.Count(b => b.Status == BookingStatus.Cancelled),
                TotalSpent = bookings.Where(b => b.PaymentStatus == PaymentStatus.Paid).Sum(b => b.FinalAmount)
            });
        }
        catch (Exception ex) { _logger.LogError(ex, "Error stats"); return StatusCode(500, new { message = "Error" }); }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrder(Guid id)
    {
        try
        {
            var userId = ResolveUserId();
            var order = await _context.Bookings
                .Include(b => b.Hotel).Include(b => b.HotelRoom).Include(b => b.Resort).Include(b => b.ResortRoom)
                .Include(b => b.TourPackage).Include(b => b.Tour).Include(b => b.Transport).Include(b => b.Restaurant)
                .Include(b => b.Promotion).FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (order == null) return NotFound(new { message = "Not found" });

            var svc = BuildServiceInfo(order);
            var policy = BuildCancellationPolicy(order);

            return Ok(new OrderDetailDto
            {
                OrderId = order.Id, OrderCode = order.BookingCode, Status = order.Status.ToString(),
                BookingDate = order.CreatedAt, ServiceDate = GetServiceDate(order), EndDate = order.EndDate,
                Quantity = order.Quantity, ExpiredAt = order.ExpiredAt ?? DateTime.UtcNow.AddDays(1), UserId = order.UserId,
                Service = svc,
                Payment = new OrderPaymentInfoDto { PaymentStatus = order.PaymentStatus.ToString(), PaymentMethod = order.PaymentMethod,
                    TotalAmount = order.TotalAmount, DiscountAmount = order.DiscountAmount ?? 0, FinalAmount = order.FinalAmount,
                    PromotionCode = order.Promotion?.Code, PaidAt = order.PaidAt },
                Customer = new OrderCustomerInfoDto { ContactName = order.ContactName, ContactEmail = order.ContactEmail,
                    ContactPhone = order.ContactPhone, Notes = order.Notes },
                CancellationPolicy = policy,
                CanReview = order.Status == BookingStatus.Completed && order.PaymentStatus == PaymentStatus.Paid
            });
        }
        catch (Exception ex) { _logger.LogError(ex, "Error detail"); return StatusCode(500, new { message = "Error" }); }
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<OrderActionResult>> CancelOrder(Guid id, [FromBody] CancelOrderRequest req)
    {
        try
        {
            var order = await _context.Bookings.Include(b => b.Promotion).FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (order == null) return NotFound(new { message = "Not found" });
            if (order.Status == BookingStatus.Cancelled || order.Status == BookingStatus.Completed)
                return BadRequest(new OrderActionResult { Success = false, Message = "Cannot cancel", OrderId = id });
            
            var serviceDate = GetServiceDate(order);
            if (DateTime.UtcNow > serviceDate.AddHours(-48))
                return BadRequest(new OrderActionResult { Success = false, Message = "Deadline passed", OrderId = id });

            order.Status = BookingStatus.Cancelled;
            order.CancellationReason = req.Reason ?? "User cancelled";
            order.CancelledAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            if (order.PaymentStatus == PaymentStatus.Paid) order.PaymentStatus = PaymentStatus.Refunded;
            await _context.SaveChangesAsync();
            return Ok(new OrderActionResult { Success = true, Message = "Cancelled", OrderId = id, NewStatus = "Cancelled" });
        }
        catch (Exception ex) { _logger.LogError(ex, "Cancel error"); return StatusCode(500, new OrderActionResult { Success = false, Message = "Error", OrderId = id }); }
    }

    [HttpPatch("{id:guid}/change-date")]
    public async Task<ActionResult<OrderActionResult>> ChangeDate(Guid id, [FromBody] ChangeDateRequest req)
    {
        try
        {
            var order = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (order == null) return NotFound(new { message = "Not found" });
            if (order.Status == BookingStatus.Cancelled || order.Status == BookingStatus.Completed)
                return BadRequest(new OrderActionResult { Success = false, Message = "Cannot change", OrderId = id });
            
            var serviceDate = GetServiceDate(order);
            if (DateTime.UtcNow > serviceDate.AddHours(-48))
                return BadRequest(new OrderActionResult { Success = false, Message = "Deadline passed", OrderId = id });
            if (req.NewServiceDate <= DateTime.UtcNow)
                return BadRequest(new OrderActionResult { Success = false, Message = "Invalid date", OrderId = id });

            var oldDate = serviceDate;
            order.ServiceDate = req.NewServiceDate;
            if (req.NewEndDate != null) order.EndDate = req.NewEndDate;
            order.Notes = (order.Notes ?? "") + $"\n[Date change] Old: {oldDate:dd/MM/yyyy}, Reason: {req.Reason ?? "None"}";
            order.UpdatedAt = DateTime.UtcNow;
            order.Status = BookingStatus.Pending;
            await _context.SaveChangesAsync();
            return Ok(new OrderActionResult { Success = true, Message = "Date change requested", OrderId = id, NewStatus = "Pending" });
        }
        catch (Exception ex) { _logger.LogError(ex, "Change date error"); return StatusCode(500, new OrderActionResult { Success = false, Message = "Error", OrderId = id }); }
    }

    [HttpGet("{id:guid}/invoice")]
    public async Task<IActionResult> GetInvoice(Guid id)
    {
        try
        {
            var order = await _context.Bookings
                .Include(b => b.Hotel).Include(b => b.HotelRoom).Include(b => b.Resort).Include(b => b.ResortRoom)
                .Include(b => b.TourPackage).Include(b => b.Tour).Include(b => b.Transport).Include(b => b.Restaurant)
                .Include(b => b.Promotion).FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (order == null) return NotFound(new { message = "Not found" });

            var svc = BuildServiceInfo(order);
            var invoice = new InvoiceDto
            {
                InvoiceNumber = $"INV-{order.BookingCode}", InvoiceDate = DateTime.UtcNow,
                CustomerName = order.ContactName ?? "Customer", CustomerEmail = order.ContactEmail ?? "", CustomerPhone = order.ContactPhone ?? "",
                OrderCode = order.BookingCode, BookingDate = order.CreatedAt, ServiceDate = GetServiceDate(order), EndDate = order.EndDate,
                ServiceType = order.Type.ToString(), ServiceName = svc.ServiceName, ServiceAddress = svc.ServiceAddress,
                ServiceDetails = svc.RoomType ?? svc.TransportRoute ?? svc.TourDestination,
                Quantity = order.Quantity, UnitPrice = order.Quantity > 0 ? order.TotalAmount / order.Quantity : order.TotalAmount,
                TotalAmount = order.TotalAmount, DiscountAmount = order.DiscountAmount ?? 0, FinalAmount = order.FinalAmount,
                PromotionCode = order.Promotion?.Code, PaymentStatus = order.PaymentStatus.ToString(), PaymentMethod = order.PaymentMethod, PaidAt = order.PaidAt
            };
            var pdf = _invoiceService.GenerateInvoicePdf(invoice);
            return File(pdf, "application/pdf", $"Invoice_{invoice.InvoiceNumber}.pdf");
        }
        catch (Exception ex) { _logger.LogError(ex, "Invoice error"); return StatusCode(500, new { message = "Error" }); }
    }

    private string GetServiceName(Booking o) => o.Type switch
    {
        BookingType.Hotel => o.HotelId != Guid.Empty ? (_context.Hotels.Find(o.HotelId)?.Name ?? "Hotel") : "Hotel",
        BookingType.Tour => o.TourPackageId != Guid.Empty ? (_context.TourPackages.Find(o.TourPackageId)?.Name ?? "Tour") : "Tour",
        BookingType.Resort => o.ResortId != Guid.Empty ? (_context.Resorts.Find(o.ResortId)?.Name ?? "Resort") : "Resort",
        BookingType.Transport => o.TransportId != Guid.Empty ? (_context.Transports.Find(o.TransportId)?.Provider ?? "Transport") : "Transport",
        BookingType.Restaurant => o.RestaurantId != Guid.Empty ? (_context.Restaurants.Find(o.RestaurantId)?.Name ?? "Restaurant") : "Restaurant",
        _ => "Service"
    };

    private OrderServiceInfoDto BuildServiceInfo(Booking o)
    {
        var info = new OrderServiceInfoDto { ServiceType = o.Type.ToString() };
        switch (o.Type)
        {
            case BookingType.Hotel:
                if (o.HotelId != Guid.Empty) { var h = _context.Hotels.Find(o.HotelId); if (h != null) { info.ServiceId = h.Id; info.ServiceName = h.Name; info.ServiceAddress = h.Address; info.ServiceImage = h.Images; } }
                if (o.HotelRoomId != Guid.Empty) { var r = _context.HotelRooms.Find(o.HotelRoomId); if (r != null) { info.RoomType = r.RoomType; info.MaxOccupancy = r.MaxOccupancy; } }
                break;
            case BookingType.Tour:
                if (o.TourPackageId != Guid.Empty) { var t = _context.TourPackages.Find(o.TourPackageId); if (t != null) { info.ServiceId = t.Id; info.ServiceName = t.Name; info.TourDestination = t.Destination; info.DurationDays = t.DurationDays; } }
                break;
            case BookingType.Resort:
                if (o.ResortId != Guid.Empty) { var r = _context.Resorts.Find(o.ResortId); if (r != null) { info.ServiceId = r.Id; info.ServiceName = r.Name; info.ServiceAddress = r.Address; info.ServiceImage = ExtractFirstImage(r.Images); } }
                if (o.ResortRoomId != Guid.Empty) { var rm = _context.ResortRooms.Find(o.ResortRoomId); if (rm != null) { info.RoomType = rm.RoomType; info.MaxOccupancy = rm.MaxOccupancy; } }
                break;
            case BookingType.Transport:
                if (o.TransportId != Guid.Empty) { var t = _context.Transports.Find(o.TransportId); if (t != null) { info.ServiceId = t.Id; info.ServiceName = $"{t.Provider} - {t.Route}"; info.TransportRoute = t.Route; info.TransportProvider = t.Provider; } }
                break;
            case BookingType.Restaurant:
                if (o.RestaurantId != Guid.Empty) { var r = _context.Restaurants.Find(o.RestaurantId); if (r != null) { info.ServiceId = r.Id; info.ServiceName = r.Name; info.ServiceAddress = r.Address; info.ServiceImage = ExtractFirstImage(r.Images); } }
                break;
        }
        return info;
    }

    private OrderCancellationPolicyDto BuildCancellationPolicy(Booking o)
    {
        var p = new OrderCancellationPolicyDto { CancellationReason = o.CancellationReason, CancelledAt = o.CancelledAt };
        var serviceDate = GetServiceDate(o);
        var canMod = o.Status != BookingStatus.Cancelled && o.Status != BookingStatus.Completed && DateTime.UtcNow < serviceDate.AddHours(-48);
        p.CanCancel = canMod; p.CanChangeDate = canMod; p.DeadlineToCancel = serviceDate.AddHours(-48);
        if (canMod && o.PaymentStatus == PaymentStatus.Paid)
        {
            if (DateTime.UtcNow < serviceDate.AddHours(-24)) { p.CancellationFeePercent = 0; p.RefundAmount = o.FinalAmount; p.PolicyDescription = "Cancel >24h: 100% refund"; }
            else { p.CancellationFeePercent = 50; p.RefundAmount = o.FinalAmount * 0.5m; p.PolicyDescription = "Cancel <24h: 50% fee"; }
        }
        else if (!canMod) { p.PolicyDescription = "Deadline passed (48h before service)"; }
        return p;
    }

    private string? ExtractFirstImage(string? jsonImages)
    {
        if (string.IsNullOrEmpty(jsonImages)) return null;
        try
        {
            var images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(jsonImages);
            return images?.FirstOrDefault();
        }
        catch { return null; }
    }
}
