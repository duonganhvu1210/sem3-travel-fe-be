namespace KarnelTravels.API.DTOs;

// ==================== Enums for Orders ====================
public enum OrderStatusFilter
{
    All = 0,
    Pending = 1,      // Chờ xác nhận
    Confirmed = 2,    // Đã xác nhận
    Completed = 3,    // Đã hoàn thành
    Cancelled = 4     // Đã hủy
}

public enum ServiceTypeFilter
{
    All = 0,
    Tour = 1,
    Hotel = 2,
    Resort = 3,
    Transport = 4,
    Restaurant = 5
}

// ==================== Order List DTOs ====================

/// <summary>
/// DTO for paginated order list response
/// </summary>
public class OrderListDto
{
    public Guid OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO for paginated list response with metadata
/// </summary>
public class PagedOrderResult
{
    public List<OrderListDto> Orders { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Request parameters for filtering orders
/// </summary>
public class OrderFilterRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public OrderStatusFilter? Status { get; set; }
    public ServiceTypeFilter? ServiceType { get; set; }
    public string? SearchQuery { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SortBy { get; set; }        // "BookingDate", "ServiceDate", "TotalPrice"
    public bool SortDescending { get; set; } = true;
}

// ==================== Order Detail DTOs ====================

/// <summary>
/// Service information embedded in order detail
/// </summary>
public class OrderServiceInfoDto
{
    public string ServiceType { get; set; } = string.Empty;
    public Guid? ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? ServiceImage { get; set; }
    public string? ServiceAddress { get; set; }
    public string? RoomType { get; set; }           // For hotels/resorts
    public int? MaxOccupancy { get; set; }
    public string? TransportRoute { get; set; }      // For transport
    public string? TransportProvider { get; set; }
    public string? TourDestination { get; set; }     // For tours
    public int? DurationDays { get; set; }
}

/// <summary>
/// Payment information for order detail
/// </summary>
public class OrderPaymentInfoDto
{
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? PromotionCode { get; set; }
    public DateTime? PaidAt { get; set; }
}

/// <summary>
/// Customer information for order detail
/// </summary>
public class OrderCustomerInfoDto
{
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// Cancellation policy and refund info
/// </summary>
public class OrderCancellationPolicyDto
{
    public bool CanCancel { get; set; }
    public bool CanChangeDate { get; set; }
    public decimal? CancellationFeePercent { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? DeadlineToCancel { get; set; }     // 48 hours before service
    public string? PolicyDescription { get; set; }
}

/// <summary>
/// Complete order detail response
/// </summary>
public class OrderDetailDto
{
    public Guid OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiredAt { get; set; }
    
    // Related information
    public OrderServiceInfoDto Service { get; set; } = new();
    public OrderPaymentInfoDto Payment { get; set; } = new();
    public OrderCustomerInfoDto Customer { get; set; } = new();
    public OrderCancellationPolicyDto CancellationPolicy { get; set; } = new();
    
    // Additional info
    public bool CanReview { get; set; }
    public Guid? UserId { get; set; }
}

// ==================== Order Actions ====================

/// <summary>
/// Request to cancel an order
/// </summary>
public class CancelOrderRequest
{
    public string? Reason { get; set; }
}

/// <summary>
/// Request to change service date
/// </summary>
public class ChangeDateRequest
{
    public DateTime NewServiceDate { get; set; }
    public DateTime? NewEndDate { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Response for order actions
/// </summary>
public class OrderActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public string? NewStatus { get; set; }
}

// ==================== Invoice DTOs ====================

/// <summary>
/// Invoice data for PDF generation
/// </summary>
public class InvoiceDto
{
    // Invoice Header
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string CompanyName { get; set; } = "Karnel Travels";
    public string CompanyAddress { get; set; } = "123 Nguyen Trai Street, District 1, Ho Chi Minh City";
    public string CompanyPhone { get; set; } = "1900 6677";
    public string CompanyEmail { get; set; } = "info@karneltravels.com";
    
    // Customer Info
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Order Info
    public string OrderCode { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Service Details
    public string ServiceType { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string? ServiceAddress { get; set; }
    public string? ServiceDetails { get; set; }  // Room type, transport route, etc.
    public int Quantity { get; set; }
    
    // Pricing
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? PromotionCode { get; set; }
    
    // Payment
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    
    // Footer
    public string TermsAndConditions { get; set; } = "Please contact us if you have any questions.";
    public string ThankYouMessage { get; set; } = "Thank you for using Karnel Travels services!";
}

// ==================== Order Statistics ====================

/// <summary>
/// Statistics for user orders
/// </summary>
public class OrderStatisticsDto
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TotalSpent { get; set; }
}

/// <summary>
/// DTO for coupon validation response
/// </summary>
public class CouponValidationDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public DateTime? ValidUntil { get; set; }
}
