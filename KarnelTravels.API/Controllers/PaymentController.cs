using KarnelTravels.API.DTOs;
using KarnelTravels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PaymentController : ControllerBase
{
    private readonly IVnPayService _vnPayService;
    private readonly IConfiguration _configuration;

    public PaymentController(IVnPayService vnPayService, IConfiguration configuration)
    {
        _vnPayService = vnPayService;
        _configuration = configuration;
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

            // Xử lý kết quả thanh toán
            // TODO: Cập nhật trạng thái đơn hàng trong database

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

            // TODO: Cập nhật trạng thái đơn hàng trong database

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
