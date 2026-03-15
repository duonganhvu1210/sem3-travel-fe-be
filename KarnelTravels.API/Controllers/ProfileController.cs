using System.Security.Claims;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        return userId;
    }

    /// <summary>
    /// F334: Lấy thông tin chi tiết người dùng hiện tại
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.GetProfileAsync(userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F335, F338, F339, F343: Cập nhật thông tin cơ bản, ngày sinh, giới tính, sở thích du lịch
    /// </summary>
    [HttpPut("update")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.UpdateProfileAsync(userId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F336: Xử lý Upload file ảnh đại diện
    /// </summary>
    [HttpPost("avatar")]
    public async Task<ActionResult<ApiResponse<string>>> UploadAvatar(IFormFile file)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.UploadAvatarAsync(userId, file);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<string>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F337: Thay đổi mật khẩu an toàn
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse<string>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.ChangePasswordAsync(userId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<string>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F340: Truy vấn lịch sử hoạt động
    /// </summary>
    [HttpGet("activities")]
    public async Task<ActionResult<ApiResponse<List<AccountActivityDto>>>> GetActivities([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.GetActivitiesAsync(userId, pageIndex, pageSize);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<List<AccountActivityDto>>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<AccountActivityDto>>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F341: Lấy danh sách địa chỉ
    /// </summary>
    [HttpGet("addresses")]
    public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetAddresses()
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.GetAddressesAsync(userId);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<List<AddressDto>>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<AddressDto>>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F341: Thêm địa chỉ mới
    /// </summary>
    [HttpPost("addresses")]
    public async Task<ActionResult<ApiResponse<AddressDto>>> CreateAddress([FromBody] CreateAddressRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.CreateAddressAsync(userId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<AddressDto>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F341: Cập nhật địa chỉ
    /// </summary>
    [HttpPut("addresses")]
    public async Task<ActionResult<ApiResponse<AddressDto>>> UpdateAddress([FromBody] UpdateAddressRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.UpdateAddressAsync(userId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<AddressDto>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F341: Xóa địa chỉ
    /// </summary>
    [HttpDelete("addresses/{addressId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteAddress(Guid addressId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.DeleteAddressAsync(userId, addressId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<string>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F341: Đặt địa chỉ mặc định
    /// </summary>
    [HttpPut("addresses/{addressId}/set-default")]
    public async Task<ActionResult<ApiResponse<string>>> SetDefaultAddress(Guid addressId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.SetDefaultAddressAsync(userId, addressId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<string>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// F342: Gửi lại email xác thực
    /// </summary>
    [HttpPost("resend-verification")]
    public async Task<ActionResult<ApiResponse<string>>> ResendVerification()
    {
        try
        {
            var userId = GetUserId();
            var result = await _profileService.ResendVerificationEmailAsync(userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<string>
            {
                Success = false,
                Message = "Token không hợp lệ"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }
}
