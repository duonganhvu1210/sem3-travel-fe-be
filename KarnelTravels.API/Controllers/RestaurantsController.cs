using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    // ==================== Restaurant CRUD ====================

    /// <summary>
    /// Lấy danh sách tất cả nhà hàng với phân trang (F259)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RestaurantDto>>>> GetRestaurants(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] string? city = null,
        [FromQuery] string? cuisineType = null,
        [FromQuery] string? priceRange = null)
    {
        var restaurants = await _restaurantService.GetAllAsync();
        
        // Filter by search
        if (!string.IsNullOrWhiteSpace(search))
        {
            restaurants = restaurants.Where(r => 
                r.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (r.Description != null && r.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        // Filter by city
        if (!string.IsNullOrWhiteSpace(city))
        {
            restaurants = restaurants.Where(r => r.City == city).ToList();
        }

        // Sorting
        restaurants = sortBy?.ToLower() switch
        {
            "price" => restaurants.OrderBy(r => r.PriceLevel).ToList(),
            "price_desc" => restaurants.OrderByDescending(r => r.PriceLevel).ToList(),
            "rating" => restaurants.OrderByDescending(r => r.Rating).ToList(),
            "name" => restaurants.OrderBy(r => r.Name).ToList(),
            _ => restaurants.OrderByDescending(r => r.IsFeatured).ThenByDescending(r => r.Rating).ToList()
        };

        var totalCount = restaurants.Count;
        var pagedData = restaurants.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        return Ok(new ApiResponse<List<RestaurantDto>>
        {
            Success = true,
            Data = pagedData,
            Pagination = new PaginationInfo
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        });
    }

    /// <summary>
    /// Tìm kiếm và lọc nhà hàng (F271)
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RestaurantDto>>>> SearchRestaurants(
        [FromQuery] string? searchTerm,
        [FromQuery] CuisineType? cuisineType,
        [FromQuery] PriceRange? priceRange,
        [FromQuery] string? city)
    {
        var restaurants = await _restaurantService.SearchAsync(searchTerm, cuisineType, priceRange, city);
        return Ok(new ApiResponse<List<RestaurantDto>>
        {
            Data = restaurants,
            Message = "Tìm kiếm nhà hàng thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin nhà hàng theo ID (F259)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RestaurantDto>>> GetRestaurant(Guid id)
    {
        var restaurant = await _restaurantService.GetByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound(new ApiResponse<RestaurantDto>
            {
                Success = false,
                Message = "Không tìm thấy nhà hàng"
            });
        }

        return Ok(new ApiResponse<RestaurantDto>
        {
            Data = restaurant,
            Message = "Lấy thông tin nhà hàng thành công"
        });
    }

    /// <summary>
    /// Tạo mới nhà hàng (F259)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<RestaurantDto>>> CreateRestaurant([FromBody] CreateRestaurantRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<RestaurantDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        try
        {
            var restaurant = await _restaurantService.CreateAsync(request);
            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.RestaurantId }, new ApiResponse<RestaurantDto>
            {
                Data = restaurant,
                Message = "Tạo nhà hàng thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<RestaurantDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật nhà hàng (F259)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<RestaurantDto>>> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantRequest request)
    {
        try
        {
            var restaurant = await _restaurantService.UpdateAsync(id, request);
            if (restaurant == null)
            {
                return NotFound(new ApiResponse<RestaurantDto>
                {
                    Success = false,
                    Message = "Không tìm thấy nhà hàng"
                });
            }

            return Ok(new ApiResponse<RestaurantDto>
            {
                Data = restaurant,
                Message = "Cập nhật nhà hàng thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<RestaurantDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Xóa nhà hàng (F259)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRestaurant(Guid id)
    {
        var result = await _restaurantService.DeleteAsync(id);
        if (!result)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không thể xóa nhà hàng. Có đơn đặt đang chờ xử lý."
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa nhà hàng thành công"
        });
    }

    // ==================== Hours Management ====================

    /// <summary>
    /// Cập nhật giờ mở/đóng cửa (F266)
    /// </summary>
    [HttpPut("{id}/hours")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<RestaurantDto>>> UpdateHours(Guid id, [FromBody] UpdateRestaurantHoursRequest request)
    {
        try
        {
            var restaurant = await _restaurantService.UpdateHoursAsync(id, request);
            if (restaurant == null)
            {
                return NotFound(new ApiResponse<RestaurantDto>
                {
                    Success = false,
                    Message = "Không tìm thấy nhà hàng"
                });
            }

            return Ok(new ApiResponse<RestaurantDto>
            {
                Data = restaurant,
                Message = "Cập nhật giờ hoạt động thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<RestaurantDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    // ==================== Menu Management ====================

    /// <summary>
    /// Lấy thực đơn nhà hàng (F263)
    /// </summary>
    [HttpGet("{id}/menu")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<MenuItemDto>>>> GetMenu(Guid id)
    {
        var menu = await _restaurantService.GetMenuAsync(id);
        return Ok(new ApiResponse<List<MenuItemDto>>
        {
            Data = menu,
            Message = "Lấy thực đơn thành công"
        });
    }

    /// <summary>
    /// Thêm món ăn vào thực đơn (F263)
    /// </summary>
    [HttpPost("{id}/menu")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<MenuItemDto>>> CreateMenuItem(Guid id, [FromBody] CreateMenuItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<MenuItemDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        try
        {
            var item = await _restaurantService.CreateMenuItemAsync(id, request);
            return CreatedAtAction(nameof(GetMenu), new { id }, new ApiResponse<MenuItemDto>
            {
                Data = item,
                Message = "Thêm món ăn thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<MenuItemDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật món ăn (F263)
    /// </summary>
    [HttpPut("{id}/menu/{menuItemId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<MenuItemDto>>> UpdateMenuItem(Guid id, Guid menuItemId, [FromBody] UpdateMenuItemRequest request)
    {
        try
        {
            var item = await _restaurantService.UpdateMenuItemAsync(id, menuItemId, request);
            if (item == null)
            {
                return NotFound(new ApiResponse<MenuItemDto>
                {
                    Success = false,
                    Message = "Không tìm thấy món ăn"
                });
            }

            return Ok(new ApiResponse<MenuItemDto>
            {
                Data = item,
                Message = "Cập nhật món ăn thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<MenuItemDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Xóa món ăn (F263)
    /// </summary>
    [HttpDelete("{id}/menu/{menuItemId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMenuItem(Guid id, Guid menuItemId)
    {
        var result = await _restaurantService.DeleteMenuItemAsync(id, menuItemId);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy món ăn"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Xóa món ăn thành công"
        });
    }

    // ==================== Table Reservations ====================

    /// <summary>
    /// Lấy danh sách đặt bàn (F265)
    /// </summary>
    [HttpGet("{id}/reservations")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<TableReservationDto>>>> GetReservations(Guid id)
    {
        var reservations = await _restaurantService.GetReservationsAsync(id);
        return Ok(new ApiResponse<List<TableReservationDto>>
        {
            Data = reservations,
            Message = "Lấy danh sách đặt bàn thành công"
        });
    }

    /// <summary>
    /// Lấy thông tin đặt bàn theo ID (F265)
    /// </summary>
    [HttpGet("{id}/reservations/{reservationId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<TableReservationDto>>> GetReservation(Guid id, Guid reservationId)
    {
        var reservation = await _restaurantService.GetReservationByIdAsync(id, reservationId);
        if (reservation == null)
        {
            return NotFound(new ApiResponse<TableReservationDto>
            {
                Success = false,
                Message = "Không tìm thấy đặt bàn"
            });
        }

        return Ok(new ApiResponse<TableReservationDto>
        {
            Data = reservation,
            Message = "Lấy thông tin đặt bàn thành công"
        });
    }

    /// <summary>
    /// Tạo đặt bàn mới (F265)
    /// </summary>
    [HttpPost("{id}/reservations")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TableReservationDto>>> CreateReservation([FromBody] CreateTableReservationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<TableReservationDto>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        try
        {
            var reservation = await _restaurantService.CreateReservationAsync(request);
            return CreatedAtAction(nameof(GetReservation), new { id = request.RestaurantId, reservationId = reservation.ReservationId }, new ApiResponse<TableReservationDto>
            {
                Data = reservation,
                Message = "Đặt bàn thành công"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<TableReservationDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Cập nhật trạng thái đặt bàn (F265)
    /// </summary>
    [HttpPut("{id}/reservations/{reservationId}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<TableReservationDto>>> UpdateReservationStatus(
        Guid id, 
        Guid reservationId, 
        [FromBody] UpdateReservationStatusRequest request)
    {
        var reservation = await _restaurantService.UpdateReservationStatusAsync(id, reservationId, request);
        if (reservation == null)
        {
            return NotFound(new ApiResponse<TableReservationDto>
            {
                Success = false,
                Message = "Không tìm thấy đặt bàn"
            });
        }

        return Ok(new ApiResponse<TableReservationDto>
        {
            Data = reservation,
            Message = "Cập nhật trạng thái thành công"
        });
    }

    /// <summary>
    /// Hủy đặt bàn (F265)
    /// </summary>
    [HttpDelete("{id}/reservations/{reservationId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteReservation(Guid id, Guid reservationId)
    {
        var result = await _restaurantService.DeleteReservationAsync(id, reservationId);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Không tìm thấy đặt bàn"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Data = true,
            Message = "Hủy đặt bàn thành công"
        });
    }
}
