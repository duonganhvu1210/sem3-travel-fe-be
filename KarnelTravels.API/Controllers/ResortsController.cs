using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ResortsController : ControllerBase
{
    private readonly IResortService _resortService;

    public ResortsController(IResortService resortService)
    {
        _resortService = resortService;
    }

    // ==================== Resort CRUD ====================

    /// <summary>
    /// Lấy danh sách tất cả resort (F272)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<ResortDto>>>> GetResorts()
    {
        var resorts = await _resortService.GetAllAsync();
        return Ok(new ApiResponse<List<ResortDto>>
        {
            Data = resorts,
            Message = "Lấy danh sách resort thành công"
        });
    }

    /// <summary>
    /// Tìm kiếm và lọc resort (F286)
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<ResortDto>>>> SearchResorts(
        [FromQuery] string? searchTerm,
        [FromQuery] ResortType? resortType,
        [FromQuery] List<ResortAmenity>? amenities,
        [FromQuery] string? city)
    {
        var resorts = await _resortService.SearchAsync(searchTerm, resortType, amenities, city);
        return Ok(new ApiResponse<List<ResortDto>>
        {
            Data = resorts,
            Message = "Tìm kiếm resort thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin resort theo ID (F272)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ResortDto>>> GetResort(Guid id)
    {
        var resort = await _resortService.GetByIdAsync(id);
        if (resort == null)
        {
            return NotFound(new ApiResponse<ResortDto>
            {
                Success = false,
                Message = "Không tìm thấy resort"
            });
        }

        return Ok(new ApiResponse<ResortDto>
        {
            Data = resort,
            Message = "Lấy thông tin resort thành công"
        });
    }

    /// <summary>
    /// Tạo mới resort (F272)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ResortDto>>> CreateResort([FromBody] CreateResortRequest request)
    {
        try
        {
            // Manual validation
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new ApiResponse<ResortDto>
                {
                    Success = false,
                    Message = "Tên resort là bắt buộc"
                });
            }

            if (string.IsNullOrWhiteSpace(request.City))
            {
                return BadRequest(new ApiResponse<ResortDto>
                {
                    Success = false,
                    Message = "Thành phố là bắt buộc"
                });
            }

            var resort = await _resortService.CreateAsync(request);
            return CreatedAtAction(nameof(GetResort), new { id = resort.ResortId }, new ApiResponse<ResortDto>
            {
                Success = true,
                Data = resort,
                Message = "Tạo resort thành công"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<ResortDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Cập nhật resort (F272)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ResortDto>>> UpdateResort(Guid id, [FromBody] UpdateResortRequest request)
    {
        try
        {
            var resort = await _resortService.UpdateAsync(id, request);
            if (resort == null)
            {
                return NotFound(new ApiResponse<ResortDto>
                {
                    Success = false,
                    Message = "Không tìm thấy resort"
                });
            }

            return Ok(new ApiResponse<ResortDto>
            {
                Success = true,
                Data = resort,
                Message = "Cập nhật resort thành công"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<ResortDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Xóa resort (F272)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteResort(Guid id)
    {
        var result = await _resortService.DeleteAsync(id);
        if (!result)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không thể xóa resort. Có đơn đặt đang chờ xử lý."
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa resort thành công"
        });
    }

    // ==================== Room Management ====================

    /// <summary>
    /// Lấy danh sách phòng (F276)
    /// </summary>
    [HttpGet("{id}/rooms")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<ResortRoomDto>>>> GetRooms(Guid id)
    {
        var rooms = await _resortService.GetRoomsAsync(id);
        return Ok(new ApiResponse<List<ResortRoomDto>>
        {
            Data = rooms,
            Message = "Lấy danh sách phòng thành công"
        });
    }

    /// <summary>
    /// Thêm phòng mới (F276)
    /// </summary>
    [HttpPost("{id}/rooms")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ResortRoomDto>>> CreateRoom(Guid id, [FromBody] CreateResortRoomRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<ResortRoomDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        try
        {
            var room = await _resortService.CreateRoomAsync(id, request);
            return CreatedAtAction(nameof(GetRooms), new { id }, new ApiResponse<ResortRoomDto>
            {
                Data = room,
                Message = "Thêm phòng thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<ResortRoomDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật phòng (F277)
    /// </summary>
    [HttpPut("{id}/rooms/{roomId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ResortRoomDto>>> UpdateRoom(Guid id, Guid roomId, [FromBody] UpdateResortRoomRequest request)
    {
        try
        {
            var room = await _resortService.UpdateRoomAsync(id, roomId, request);
            if (room == null)
            {
                return NotFound(new ApiResponse<ResortRoomDto>
                {
                    Success = false,
                    Message = "Không tìm thấy phòng"
                });
            }

            return Ok(new ApiResponse<ResortRoomDto>
            {
                Data = room,
                Message = "Cập nhật phòng thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<ResortRoomDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Xóa phòng (F277)
    /// </summary>
    [HttpDelete("{id}/rooms/{roomId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRoom(Guid id, Guid roomId)
    {
        var result = await _resortService.DeleteRoomAsync(id, roomId);
        if (!result)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không thể xóa phòng. Có đơn đặt đang chờ xử lý."
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa phòng thành công"
        });
    }

    // ==================== Room Availability & Pricing ====================

    /// <summary>
    /// Lấy tình trạng phòng theo ngày (F277)
    /// </summary>
    [HttpGet("{id}/rooms/{roomId}/availability")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RoomAvailabilityDto>>>> GetRoomAvailability(
        Guid id, 
        Guid roomId,
        [FromQuery] DateTime startDate = default,
        [FromQuery] int days = 30)
    {
        if (startDate == default)
            startDate = DateTime.Today;
            
        var availability = await _resortService.GetRoomAvailabilityAsync(id, roomId, startDate, days);
        return Ok(new ApiResponse<List<RoomAvailabilityDto>>
        {
            Data = availability,
            Message = "Lấy tình trạng phòng thành công"
        });
    }

    /// <summary>
    /// Cập nhật số lượng phòng trống (F278)
    /// </summary>
    [HttpPut("{id}/rooms/{roomId}/availability")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateRoomAvailability(
        Guid id, 
        Guid roomId, 
        [FromBody] UpdateRoomAvailabilityRequest request)
    {
        var result = await _resortService.UpdateRoomAvailabilityAsync(id, roomId, request);
        if (!result)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy phòng"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Cập nhật tình trạng phòng thành công"
        });
    }

    /// <summary>
    /// Cập nhật giá theo ngày (F278)
    /// </summary>
    [HttpPut("{id}/rooms/{roomId}/pricing")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> BulkUpdateRoomAvailability(
        Guid id, 
        Guid roomId, 
        [FromBody] BulkUpdateRoomAvailabilityRequest request)
    {
        var result = await _resortService.BulkUpdateRoomAvailabilityAsync(id, roomId, request);
        if (!result)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy phòng"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Cập nhật giá theo ngày thành công"
        });
    }

    /// <summary>
    /// Lấy giá phòng theo ngày (F278)
    /// </summary>
    [HttpGet("{id}/rooms/{roomId}/pricing")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RoomPricingDto>>>> GetRoomPricing(
        Guid id, 
        Guid roomId,
        [FromQuery] DateTime startDate = default,
        [FromQuery] int days = 30)
    {
        if (startDate == default)
            startDate = DateTime.Today;
            
        var pricing = await _resortService.GetRoomPricingAsync(id, roomId, startDate, days);
        return Ok(new ApiResponse<List<RoomPricingDto>>
        {
            Data = pricing,
            Message = "Lấy giá phòng thành công"
        });
    }

    // ==================== Resort Services (Activities) ====================

    /// <summary>
    /// Lấy danh sách dịch vụ (F282)
    /// </summary>
    [HttpGet("{id}/services")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<ResortServiceDto>>>> GetServices(Guid id)
    {
        var services = await _resortService.GetServicesAsync(id);
        return Ok(new ApiResponse<List<ResortServiceDto>>
        {
            Data = services,
            Message = "Lấy danh sách dịch vụ thành công"
        });
    }

    /// <summary>
    /// Thêm dịch vụ mới (F282)
    /// </summary>
    [HttpPost("{id}/services")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ResortServiceDto>>> CreateService(Guid id, [FromBody] CreateResortServiceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<ResortServiceDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        try
        {
            var service = await _resortService.CreateServiceAsync(id, request);
            return CreatedAtAction(nameof(GetServices), new { id }, new ApiResponse<ResortServiceDto>
            {
                Data = service,
                Message = "Thêm dịch vụ thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<ResortServiceDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật dịch vụ (F282)
    /// </summary>
    [HttpPut("{id}/services/{serviceId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ResortServiceDto>>> UpdateService(
        Guid id, 
        Guid serviceId, 
        [FromBody] UpdateResortServiceRequest request)
    {
        var service = await _resortService.UpdateServiceAsync(id, serviceId, request);
        if (service == null)
        {
            return NotFound(new ApiResponse<ResortServiceDto>
            {
                Success = false,
                Message = "Không tìm thấy dịch vụ"
            });
        }

        return Ok(new ApiResponse<ResortServiceDto>
        {
            Data = service,
            Message = "Cập nhật dịch vụ thành công"
        });
    }

    /// <summary>
    /// Xóa dịch vụ (F282)
    /// </summary>
    [HttpDelete("{id}/services/{serviceId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteService(Guid id, Guid serviceId)
    {
        var result = await _resortService.DeleteServiceAsync(id, serviceId);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy dịch vụ"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa dịch vụ thành công"
        });
    }

    // ==================== Combo Packages ====================

    /// <summary>
    /// Lấy danh sách gói combo (F283)
    /// </summary>
    [HttpGet("{id}/packages")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<ComboPackageDto>>>> GetPackages(Guid id)
    {
        var packages = await _resortService.GetPackagesAsync(id);
        return Ok(new ApiResponse<List<ComboPackageDto>>
        {
            Data = packages,
            Message = "Lấy danh sách gói combo thành công"
        });
    }

    /// <summary>
    /// Tạo gói combo mới (F283)
    /// </summary>
    [HttpPost("{id}/packages")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ComboPackageDto>>> CreatePackage(Guid id, [FromBody] CreateComboPackageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<ComboPackageDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        try
        {
            var package = await _resortService.CreatePackageAsync(id, request);
            return CreatedAtAction(nameof(GetPackages), new { id }, new ApiResponse<ComboPackageDto>
            {
                Data = package,
                Message = "Tạo gói combo thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<ComboPackageDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật gói combo (F283)
    /// </summary>
    [HttpPut("{id}/packages/{packageId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ComboPackageDto>>> UpdatePackage(
        Guid id, 
        Guid packageId, 
        [FromBody] UpdateComboPackageRequest request)
    {
        try
        {
            var package = await _resortService.UpdatePackageAsync(id, packageId, request);
            if (package == null)
            {
                return NotFound(new ApiResponse<ComboPackageDto>
                {
                    Success = false,
                    Message = "Không tìm thấy gói combo"
                });
            }

            return Ok(new ApiResponse<ComboPackageDto>
            {
                Data = package,
                Message = "Cập nhật gói combo thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<ComboPackageDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Xóa gói combo (F283)
    /// </summary>
    [HttpDelete("{id}/packages/{packageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePackage(Guid id, Guid packageId)
    {
        var result = await _resortService.DeletePackageAsync(id, packageId);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy gói combo"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa gói combo thành công"
        });
    }
}
