using System.Security.Claims;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminVehiclesController : ControllerBase
{
    private readonly IVehicleTypeService _vehicleTypeService;
    private readonly ITransportProviderService _transportProviderService;
    private readonly IRouteService _routeService;
    private readonly IVehicleService _vehicleService;
    private readonly IScheduleService _scheduleService;

    public AdminVehiclesController(
        IVehicleTypeService vehicleTypeService,
        ITransportProviderService transportProviderService,
        IRouteService routeService,
        IVehicleService vehicleService,
        IScheduleService scheduleService)
    {
        _vehicleTypeService = vehicleTypeService;
        _transportProviderService = transportProviderService;
        _routeService = routeService;
        _vehicleService = vehicleService;
        _scheduleService = scheduleService;
    }

    // ==================== VEHICLE TYPES ====================

    /// <summary>
    /// Lấy danh sách tất cả loại phương tiện (F239)
    /// </summary>
    [HttpGet("vehicle-types")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<VehicleTypeDto>>>> GetVehicleTypes()
    {
        var vehicleTypes = await _vehicleTypeService.GetAllAsync();
        return Ok(new ApiResponse<List<VehicleTypeDto>>
        {
            Data = vehicleTypes,
            Message = "Lấy danh sách loại phương tiện thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin loại phương tiện theo ID
    /// </summary>
    [HttpGet("vehicle-types/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<VehicleTypeDto>>> GetVehicleType(Guid id)
    {
        var vehicleType = await _vehicleTypeService.GetByIdAsync(id);
        if (vehicleType == null)
        {
            return NotFound(new ApiResponse<VehicleTypeDto>
            {
                Success = false,
                Message = "Không tìm thấy loại phương tiện"
            });
        }

        return Ok(new ApiResponse<VehicleTypeDto>
        {
            Data = vehicleType,
            Message = "Lấy thông tin loại phương tiện thành công"
        });
    }

    /// <summary>
    /// Tạo mới loại phương tiện (F239)
    /// </summary>
    [HttpPost("vehicle-types")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<VehicleTypeDto>>> CreateVehicleType([FromBody] CreateVehicleTypeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<VehicleTypeDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var vehicleType = await _vehicleTypeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetVehicleType), new { id = vehicleType.VehicleTypeId }, new ApiResponse<VehicleTypeDto>
        {
            Data = vehicleType,
            Message = "Tạo loại phương tiện thành công"
        });
    }

    /// <summary>
    /// Cập nhật loại phương tiện
    /// </summary>
    [HttpPut("vehicle-types/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<VehicleTypeDto>>> UpdateVehicleType(Guid id, [FromBody] CreateVehicleTypeRequest request)
    {
        var vehicleType = await _vehicleTypeService.UpdateAsync(id, request);
        if (vehicleType == null)
        {
            return NotFound(new ApiResponse<VehicleTypeDto>
            {
                Success = false,
                Message = "Không tìm thấy loại phương tiện"
            });
        }

        return Ok(new ApiResponse<VehicleTypeDto>
        {
            Data = vehicleType,
            Message = "Cập nhật loại phương tiện thành công"
        });
    }

    /// <summary>
    /// Xóa loại phương tiện
    /// </summary>
    [HttpDelete("vehicle-types/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteVehicleType(Guid id)
    {
        var result = await _vehicleTypeService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy loại phương tiện"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa loại phương tiện thành công"
        });
    }

    // ==================== TRANSPORT PROVIDERS ====================

    /// <summary>
    /// Lấy danh sách tất cả nhà xe/hãng vận chuyển (F245)
    /// </summary>
    [HttpGet("providers")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<TransportProviderDto>>>> GetProviders()
    {
        var providers = await _transportProviderService.GetAllAsync();
        return Ok(new ApiResponse<List<TransportProviderDto>>
        {
            Data = providers,
            Message = "Lấy danh sách nhà vận chuyển thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin nhà vận chuyển theo ID
    /// </summary>
    [HttpGet("providers/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<TransportProviderDto>>> GetProvider(Guid id)
    {
        var provider = await _transportProviderService.GetByIdAsync(id);
        if (provider == null)
        {
            return NotFound(new ApiResponse<TransportProviderDto>
            {
                Success = false,
                Message = "Không tìm thấy nhà vận chuyển"
            });
        }

        return Ok(new ApiResponse<TransportProviderDto>
        {
            Data = provider,
            Message = "Lấy thông tin nhà vận chuyển thành công"
        });
    }

    /// <summary>
    /// Tạo mới nhà vận chuyển (F245)
    /// </summary>
    [HttpPost("providers")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<TransportProviderDto>>> CreateProvider([FromBody] CreateTransportProviderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<TransportProviderDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var provider = await _transportProviderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetProvider), new { id = provider.ProviderId }, new ApiResponse<TransportProviderDto>
        {
            Data = provider,
            Message = "Tạo nhà vận chuyển thành công"
        });
    }

    /// <summary>
    /// Cập nhật nhà vận chuyển
    /// </summary>
    [HttpPut("providers/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<TransportProviderDto>>> UpdateProvider(Guid id, [FromBody] CreateTransportProviderRequest request)
    {
        var provider = await _transportProviderService.UpdateAsync(id, request);
        if (provider == null)
        {
            return NotFound(new ApiResponse<TransportProviderDto>
            {
                Success = false,
                Message = "Không tìm thấy nhà vận chuyển"
            });
        }

        return Ok(new ApiResponse<TransportProviderDto>
        {
            Data = provider,
            Message = "Cập nhật nhà vận chuyển thành công"
        });
    }

    /// <summary>
    /// Xóa nhà vận chuyển
    /// </summary>
    [HttpDelete("providers/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProvider(Guid id)
    {
        var result = await _transportProviderService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy nhà vận chuyển"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa nhà vận chuyển thành công"
        });
    }

    // ==================== ROUTES ====================

    /// <summary>
    /// Lấy danh sách tất cả tuyến đường (F240)
    /// </summary>
    [HttpGet("routes")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<RouteDto>>>> GetRoutes()
    {
        var routes = await _routeService.GetAllAsync();
        return Ok(new ApiResponse<List<RouteDto>>
        {
            Data = routes,
            Message = "Lấy danh sách tuyến đường thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin tuyến đường theo ID
    /// </summary>
    [HttpGet("routes/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<RouteDto>>> GetRoute(Guid id)
    {
        var route = await _routeService.GetByIdAsync(id);
        if (route == null)
        {
            return NotFound(new ApiResponse<RouteDto>
            {
                Success = false,
                Message = "Không tìm thấy tuyến đường"
            });
        }

        return Ok(new ApiResponse<RouteDto>
        {
            Data = route,
            Message = "Lấy thông tin tuyến đường thành công"
        });
    }

    /// <summary>
    /// Tạo mới tuyến đường (F240)
    /// </summary>
    [HttpPost("routes")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<RouteDto>>> CreateRoute([FromBody] CreateRouteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<RouteDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var route = await _routeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetRoute), new { id = route.RouteId }, new ApiResponse<RouteDto>
        {
            Data = route,
            Message = "Tạo tuyến đường thành công"
        });
    }

    /// <summary>
    /// Cập nhật tuyến đường
    /// </summary>
    [HttpPut("routes/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<RouteDto>>> UpdateRoute(Guid id, [FromBody] CreateRouteRequest request)
    {
        var route = await _routeService.UpdateAsync(id, request);
        if (route == null)
        {
            return NotFound(new ApiResponse<RouteDto>
            {
                Success = false,
                Message = "Không tìm thấy tuyến đường"
            });
        }

        return Ok(new ApiResponse<RouteDto>
        {
            Data = route,
            Message = "Cập nhật tuyến đường thành công"
        });
    }

    /// <summary>
    /// Xóa tuyến đường
    /// </summary>
    [HttpDelete("routes/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRoute(Guid id)
    {
        var result = await _routeService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy tuyến đường"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa tuyến đường thành công"
        });
    }

    // ==================== VEHICLES ====================

    /// <summary>
    /// Lấy danh sách tất cả phương tiện (F235)
    /// </summary>
    [HttpGet("vehicles")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<VehicleDto>>>> GetVehicles()
    {
        var vehicles = await _vehicleService.GetAllAsync();
        return Ok(new ApiResponse<List<VehicleDto>>
        {
            Data = vehicles,
            Message = "Lấy danh sách phương tiện thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin phương tiện theo ID
    /// </summary>
    [HttpGet("vehicles/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<VehicleDto>>> GetVehicle(Guid id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);
        if (vehicle == null)
        {
            return NotFound(new ApiResponse<VehicleDto>
            {
                Success = false,
                Message = "Không tìm thấy phương tiện"
            });
        }

        return Ok(new ApiResponse<VehicleDto>
        {
            Data = vehicle,
            Message = "Lấy thông tin phương tiện thành công"
        });
    }

    /// <summary>
    /// Lấy danh sách phương tiện theo nhà vận chuyển
    /// </summary>
    [HttpGet("vehicles/by-provider/{providerId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<VehicleDto>>>> GetVehiclesByProvider(Guid providerId)
    {
        var vehicles = await _vehicleService.GetByProviderAsync(providerId);
        return Ok(new ApiResponse<List<VehicleDto>>
        {
            Data = vehicles,
            Message = "Lấy danh sách phương tiện thành công"
        });
    }

    /// <summary>
    /// Tạo mới phương tiện (F236)
    /// </summary>
    [HttpPost("vehicles")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<VehicleDto>>> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<VehicleDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var vehicle = await _vehicleService.CreateAsync(request);
        return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.VehicleId }, new ApiResponse<VehicleDto>
        {
            Data = vehicle,
            Message = "Tạo phương tiện thành công"
        });
    }

    /// <summary>
    /// Cập nhật phương tiện (F237)
    /// </summary>
    [HttpPut("vehicles/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<VehicleDto>>> UpdateVehicle(Guid id, [FromBody] CreateVehicleRequest request)
    {
        var vehicle = await _vehicleService.UpdateAsync(id, request);
        if (vehicle == null)
        {
            return NotFound(new ApiResponse<VehicleDto>
            {
                Success = false,
                Message = "Không tìm thấy phương tiện"
            });
        }

        return Ok(new ApiResponse<VehicleDto>
        {
            Data = vehicle,
            Message = "Cập nhật phương tiện thành công"
        });
    }

    /// <summary>
    /// Xóa phương tiện (F238)
    /// </summary>
    [HttpDelete("vehicles/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteVehicle(Guid id)
    {
        var result = await _vehicleService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy phương tiện"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa phương tiện thành công"
        });
    }

    /// <summary>
    /// Cập nhật trạng thái phương tiện (F244)
    /// </summary>
    [HttpPatch("vehicles/{id}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateVehicleStatus(Guid id, [FromBody] UpdateVehicleStatusRequest request)
    {
        var result = await _vehicleService.UpdateStatusAsync(id, request.Status);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy phương tiện"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Cập nhật trạng thái phương tiện thành công"
        });
    }

    // ==================== SCHEDULES ====================

    /// <summary>
    /// Lấy danh sách tất cả lịch trình (F241)
    /// </summary>
    [HttpGet("schedules")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<ScheduleDto>>>> GetSchedules()
    {
        var schedules = await _scheduleService.GetAllAsync();
        return Ok(new ApiResponse<List<ScheduleDto>>
        {
            Data = schedules,
            Message = "Lấy danh sách lịch trình thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin lịch trình theo ID
    /// </summary>
    [HttpGet("schedules/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ScheduleDto>>> GetSchedule(Guid id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null)
        {
            return NotFound(new ApiResponse<ScheduleDto>
            {
                Success = false,
                Message = "Không tìm thấy lịch trình"
            });
        }

        return Ok(new ApiResponse<ScheduleDto>
        {
            Data = schedule,
            Message = "Lấy thông tin lịch trình thành công"
        });
    }

    /// <summary>
    /// Lấy danh sách lịch trình theo phương tiện
    /// </summary>
    [HttpGet("schedules/by-vehicle/{vehicleId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<ScheduleDto>>>> GetSchedulesByVehicle(Guid vehicleId)
    {
        var schedules = await _scheduleService.GetByVehicleAsync(vehicleId);
        return Ok(new ApiResponse<List<ScheduleDto>>
        {
            Data = schedules,
            Message = "Lấy danh sách lịch trình thành công"
        });
    }

    /// <summary>
    /// Lấy danh sách lịch trình theo tuyến đường
    /// </summary>
    [HttpGet("schedules/by-route/{routeId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<ScheduleDto>>>> GetSchedulesByRoute(Guid routeId)
    {
        var schedules = await _scheduleService.GetByRouteAsync(routeId);
        return Ok(new ApiResponse<List<ScheduleDto>>
        {
            Data = schedules,
            Message = "Lấy danh sách lịch trình thành công"
        });
    }

    /// <summary>
    /// Tạo mới lịch trình (F241)
    /// </summary>
    [HttpPost("schedules")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ScheduleDto>>> CreateSchedule([FromBody] CreateScheduleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<ScheduleDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var schedule = await _scheduleService.CreateAsync(request);
        return CreatedAtAction(nameof(GetSchedule), new { id = schedule.ScheduleId }, new ApiResponse<ScheduleDto>
        {
            Data = schedule,
            Message = "Tạo lịch trình thành công"
        });
    }

    /// <summary>
    /// Cập nhật lịch trình
    /// </summary>
    [HttpPut("schedules/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ScheduleDto>>> UpdateSchedule(Guid id, [FromBody] UpdateScheduleRequest request)
    {
        var schedule = await _scheduleService.UpdateAsync(id, request);
        if (schedule == null)
        {
            return NotFound(new ApiResponse<ScheduleDto>
            {
                Success = false,
                Message = "Không tìm thấy lịch trình"
            });
        }

        return Ok(new ApiResponse<ScheduleDto>
        {
            Data = schedule,
            Message = "Cập nhật lịch trình thành công"
        });
    }

    /// <summary>
    /// Xóa lịch trình
    /// </summary>
    [HttpDelete("schedules/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSchedule(Guid id)
    {
        var result = await _scheduleService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy lịch trình"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa lịch trình thành công"
        });
    }
}
