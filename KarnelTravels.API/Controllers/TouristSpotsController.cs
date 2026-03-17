using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TouristSpotsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public TouristSpotsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TouristSpotDto>>>> GetTouristSpots(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] string? region = null,
        [FromQuery] string? type = null)
    {
        var query = _context.TouristSpots.Where(s => s.IsActive);

        // Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || 
                (s.Description != null && s.Description.Contains(search)) ||
                (s.City != null && s.City.Contains(search)));
        }

        // Region filter
        if (!string.IsNullOrWhiteSpace(region))
        {
            query = query.Where(s => s.Region == region);
        }

        // Type filter
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(s => s.Type == type);
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(s => s.TicketPrice),
            "price_desc" => query.OrderByDescending(s => s.TicketPrice),
            "rating" => query.OrderByDescending(s => s.Rating),
            "name" => query.OrderBy(s => s.Name),
            "name_desc" => query.OrderByDescending(s => s.Name),
            _ => query.OrderByDescending(s => s.IsFeatured).ThenByDescending(s => s.Rating)
        };

        var totalCount = await query.CountAsync();
        var spots = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = spots.Select(s => new TouristSpotDto
        {
            SpotId = s.Id,
            Name = s.Name,
            Description = s.Description,
            Region = s.Region,
            Type = s.Type,
            Address = s.Address,
            City = s.City,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            Images = string.IsNullOrEmpty(s.Images) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(s.Images),
            TicketPrice = s.TicketPrice,
            BestTime = s.BestTime,
            Rating = s.Rating,
            ReviewCount = s.ReviewCount,
            IsFeatured = s.IsFeatured
        }).ToList();

        return Ok(new ApiResponse<List<TouristSpotDto>>
        {
            Success = true,
            Data = result,
            Pagination = new PaginationInfo
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TouristSpotDto>>> GetTouristSpot(Guid id)
    {
        var spot = await _context.TouristSpots.FindAsync(id);

        if (spot == null || !spot.IsActive)
        {
            return NotFound(new ApiResponse<TouristSpotDto>
            {
                Success = false,
                Message = "Not found tourist spot"
            });
        }

        var dto = new TouristSpotDto
        {
            SpotId = spot.Id,
            Name = spot.Name,
            Description = spot.Description,
            Region = spot.Region,
            Type = spot.Type,
            Address = spot.Address,
            City = spot.City,
            Latitude = spot.Latitude,
            Longitude = spot.Longitude,
            Images = string.IsNullOrEmpty(spot.Images) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(spot.Images),
            TicketPrice = spot.TicketPrice,
            BestTime = spot.BestTime,
            Rating = spot.Rating,
            ReviewCount = spot.ReviewCount,
            IsFeatured = spot.IsFeatured
        };

        return Ok(new ApiResponse<TouristSpotDto>
        {
            Success = true,
            Data = dto
        });
    }

    [HttpGet("regions")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetRegions()
    {
        var regions = await _context.TouristSpots
            .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Region))
            .Select(s => s.Region)
            .Distinct()
            .OrderBy(r => r)
            .ToListAsync();

        return Ok(new ApiResponse<List<string>>
        {
            Success = true,
            Data = regions
        });
    }

    [HttpGet("types")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetTypes()
    {
        var types = await _context.TouristSpots
            .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Type))
            .Select(s => s.Type)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        return Ok(new ApiResponse<List<string>>
        {
            Success = true,
            Data = types
        });
    }

    // Admin: Create new tourist spot
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TouristSpotDto>>> CreateTouristSpot([FromBody] CreateTouristSpotRequest dto)
    {
        try
        {
            var spot = new TouristSpot
            {
                Name = dto.Name,
                Description = dto.Description,
                Region = dto.Region,
                Type = dto.Type,
                Address = dto.Address,
                City = dto.City,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Images = dto.Images != null ? System.Text.Json.JsonSerializer.Serialize(dto.Images) : null,
                TicketPrice = dto.TicketPrice,
                BestTime = dto.BestTime,
                IsFeatured = dto.IsFeatured,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.TouristSpots.Add(spot);
            await _context.SaveChangesAsync();

            var result = new TouristSpotDto
            {
                SpotId = spot.Id,
                Name = spot.Name,
                Description = spot.Description,
                Region = spot.Region,
                Type = spot.Type,
                Address = spot.Address,
                City = spot.City,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                Images = string.IsNullOrEmpty(spot.Images) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(spot.Images),
                TicketPrice = spot.TicketPrice,
                BestTime = spot.BestTime,
                Rating = spot.Rating,
                ReviewCount = spot.ReviewCount,
                IsFeatured = spot.IsFeatured
            };

            return Ok(new ApiResponse<TouristSpotDto>
            {
                Success = true,
                Message = "Successfully created tourist spot",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<TouristSpotDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    // Admin: Update tourist spot
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TouristSpotDto>>> UpdateTouristSpot(Guid id, [FromBody] CreateTouristSpotRequest dto)
    {
        try
        {
            var spot = await _context.TouristSpots.FindAsync(id);
            if (spot == null)
            {
                return NotFound(new ApiResponse<TouristSpotDto>
                {
                    Success = false,
                    Message = "Not found tourist spot"
                });
            }

            spot.Name = dto.Name;
            spot.Description = dto.Description;
            spot.Region = dto.Region;
            spot.Type = dto.Type;
            spot.Address = dto.Address;
            spot.City = dto.City;
            spot.Latitude = dto.Latitude;
            spot.Longitude = dto.Longitude;
            spot.Images = dto.Images != null ? System.Text.Json.JsonSerializer.Serialize(dto.Images) : null;
            spot.TicketPrice = dto.TicketPrice;
            spot.BestTime = dto.BestTime;
            spot.IsFeatured = dto.IsFeatured;
            spot.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = new TouristSpotDto
            {
                SpotId = spot.Id,
                Name = spot.Name,
                Description = spot.Description,
                Region = spot.Region,
                Type = spot.Type,
                Address = spot.Address,
                City = spot.City,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                Images = string.IsNullOrEmpty(spot.Images) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(spot.Images),
                TicketPrice = spot.TicketPrice,
                BestTime = spot.BestTime,
                Rating = spot.Rating,
                ReviewCount = spot.ReviewCount,
                IsFeatured = spot.IsFeatured
            };

            return Ok(new ApiResponse<TouristSpotDto>
            {
                Success = true,
                Message = "Successfully updated tourist spot",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<TouristSpotDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    // Admin: Delete tourist spot (soft delete)
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTouristSpot(Guid id)
    {
        try
        {
            var spot = await _context.TouristSpots.FindAsync(id);
            if (spot == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Not found tourist spot"
                });
            }

            spot.IsActive = false;
            spot.IsDeleted = true;
            spot.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Successfully deleted tourist spot",
                Data = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }
}
