using KarnelTravels.API.DTOs;
using KarnelTravels.API.Services;
using KarnelTravels.API.Data;
using KarnelTravels.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PaymentController : ControllerBase
{
    private readonly IVnPayService _vnPayService;
    private readonly IConfiguration _configuration;
    private readonly KarnelTravelsDbContext _context;

    public PaymentController(IVnPayService vnPayService, IConfiguration configuration, KarnelTravelsDbContext context)
    {
        _vnPayService = vnPayService;
        _configuration = configuration;
        _context = context;
    }

    /// <summary>
    /// Tạo URL thanh toán VNPAY
    /// </summary>
    [HttpPost("vnpay")]
    public async Task<ActionResult<ApiResponse<VnPayResponse>>> CreateVnPayPayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            // Lấy địa chỉ IP của client
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            // Tạo URL thanh toán
            var paymentUrl = _vnPayService.CreatePaymentUrl(
                request.OrderId,
                request.Amount,
                request.OrderDescription,
                clientIp
            );

            return Ok(new ApiResponse<VnPayResponse>
            {
                Success = true,
                Message = "Tạo URL thanh toán thành công",
                Data = new VnPayResponse
                {
                    PaymentUrl = paymentUrl,
                    OrderId = request.OrderId
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<VnPayResponse>
            {
                Success = false,
                Message = $"Lỗi tạo thanh toán: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Xử lý callback từ VNPAY (IPN)
    /// </summary>
    [HttpGet("vnpay/callback")]
    [AllowAnonymous]
    public async Task<ActionResult> VnPayCallback()
    {
        try
        {
            var queryString = Request.QueryString.ToString();
            var responseData = _vnPayService.GetResponseData(queryString);

            if (responseData.Count == 0)
            {
                return BadRequest(new { RspCode = "99", Message = "Input data required" });
            }

            // Validate signature
            var secureHash = Request.Query["vnp_SecureHash"];
            if (string.IsNullOrEmpty(secureHash))
            {
                return BadRequest(new { RspCode = "99", Message = "Invalid signature" });
            }

            bool isValidSignature = _vnPayService.ValidateSignature(responseData, secureHash.ToString());

            if (!isValidSignature)
            {
                return BadRequest(new { RspCode = "97", Message = "Invalid signature" });
            }

            // Lấy thông tin giao dịch
            var orderId = Guid.Parse(responseData["vnp_TxnRef"]);
            var vnpayTranId = responseData["vnp_TransactionNo"];
            var responseCode = responseData["vnp_ResponseCode"];
            var transactionStatus = responseData["vnp_TransactionStatus"];

            // Tìm và cập nhật booking
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == orderId);
            if (booking != null)
            {
                if (responseCode == "00" && transactionStatus == "00")
                {
                    // Thanh toán thành công
                    booking.PaymentStatus = PaymentStatus.Paid;
                    booking.Status = BookingStatus.Confirmed;
                    booking.PaymentMethod = "VNPAY";
                    booking.PaidAt = DateTime.UtcNow;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[Payment] Booking {booking.BookingCode} updated to Paid status");
                }
                else
                {
                    // Thanh toán thất bại
                    booking.PaymentStatus = PaymentStatus.Failed;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[Payment] Booking {booking.BookingCode} payment failed");
                }
            }
            else
            {
                Console.WriteLine($"[Payment] Booking not found: {orderId}");
            }

            if (responseCode == "00" && transactionStatus == "00")
            {
                // Thanh toán thành công
                return Ok(new { RspCode = "00", Message = "Confirm Success" });
            }
            else
            {
                // Thanh toán thất bại
                return Ok(new { RspCode = "01", Message = "Payment failed" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { RspCode = "99", Message = ex.Message });
        }
    }

    /// <summary>
    /// Xử lý kết quả trả về sau thanh toán (Return URL)
    /// </summary>
    [HttpGet("vnpay/return")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<VnPayReturnResponse>>> VnPayReturn()
    {
        try
        {
            var queryString = Request.QueryString.ToString();
            var responseData = _vnPayService.GetResponseData(queryString);

            if (responseData.Count == 0)
            {
                return BadRequest(new ApiResponse<VnPayReturnResponse>
                {
                    Success = false,
                    Message = "Không có dữ liệu trả về"
                });
            }

            // Validate signature
            var secureHash = Request.Query["vnp_SecureHash"];
            bool isValidSignature = _vnPayService.ValidateSignature(responseData, secureHash.ToString());

            if (!isValidSignature)
            {
                return Ok(new ApiResponse<VnPayReturnResponse>
                {
                    Success = false,
                    Message = "Chữ ký không hợp lệ",
                    Data = new VnPayReturnResponse
                    {
                        Success = false,
                        OrderId = responseData.ContainsKey("vnp_TxnRef") ? Guid.Parse(responseData["vnp_TxnRef"]) : Guid.Empty,
                        TransactionNo = responseData.ContainsKey("vnp_TransactionNo") ? responseData["vnp_TransactionNo"] : "",
                        ResponseCode = responseData.ContainsKey("vnp_ResponseCode") ? responseData["vnp_ResponseCode"] : ""
                    }
                });
            }

            var orderId = responseData.ContainsKey("vnp_TxnRef") ? Guid.Parse(responseData["vnp_TxnRef"]) : Guid.Empty;
            var vnpayTranId = responseData.ContainsKey("vnp_TransactionNo") ? responseData["vnp_TransactionNo"] : "";
            var responseCode = responseData.ContainsKey("vnp_ResponseCode") ? responseData["vnp_ResponseCode"] : "";
            var transactionStatus = responseData.ContainsKey("vnp_TransactionStatus") ? responseData["vnp_TransactionStatus"] : "";
            var amount = responseData.ContainsKey("vnp_Amount") ? (long.Parse(responseData["vnp_Amount"]) / 100) : 0;

            bool isSuccess = responseCode == "00" && transactionStatus == "00";

            // Cập nhật booking trong database
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == orderId);
            if (booking != null)
            {
                if (isSuccess)
                {
                    booking.PaymentStatus = PaymentStatus.Paid;
                    booking.Status = BookingStatus.Confirmed;
                    booking.PaymentMethod = "VNPAY";
                    booking.PaidAt = DateTime.UtcNow;
                }
                else
                {
                    booking.PaymentStatus = PaymentStatus.Failed;
                }
                booking.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                Console.WriteLine($"[Payment Return] Booking {booking.BookingCode} updated. Success: {isSuccess}");
            }
            else
            {
                Console.WriteLine($"[Payment Return] Booking not found: {orderId}");
            }

            return Ok(new ApiResponse<VnPayReturnResponse>
            {
                Success = isSuccess,
                Message = isSuccess ? "Thanh toán thành công" : "Thanh toán thất bại",
                Data = new VnPayReturnResponse
                {
                    Success = isSuccess,
                    OrderId = orderId,
                    TransactionNo = vnpayTranId,
                    Amount = amount,
                    ResponseCode = responseCode,
                    TransactionStatus = transactionStatus
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<VnPayReturnResponse>
            {
                Success = false,
                Message = $"Lỗi xử lý: {ex.Message}"
            });
        }
    }
}

// DTOs
public class CreatePaymentRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string OrderDescription { get; set; } = string.Empty;
}

public class VnPayResponse
{
    public string PaymentUrl { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
}

public class VnPayReturnResponse
{
    public bool Success { get; set; }
    public Guid OrderId { get; set; }
    public string TransactionNo { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string ResponseCode { get; set; } = string.Empty;
    public string TransactionStatus { get; set; } = string.Empty;
}
