using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TransportsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public TransportsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách phương tiện với phân trang và lọc
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TransportDto>>>> GetTransports(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] string? type = null,
        [FromQuery] string? fromCity = null,
        [FromQuery] string? toCity = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        var query = _context.Transports
            .Where(t => t.IsActive)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => 
                t.Provider.Contains(search) ||
                (t.Route != null && t.Route.Contains(search)) ||
                t.FromCity.Contains(search) ||
                t.ToCity.Contains(search));
        }

        // Type filter
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(t => t.Type == type);
        }

        // From city filter
        if (!string.IsNullOrWhiteSpace(fromCity))
        {
            query = query.Where(t => t.FromCity == fromCity);
        }

        // To city filter
        if (!string.IsNullOrWhiteSpace(toCity))
        {
            query = query.Where(t => t.ToCity == toCity);
        }

        // Price filters
        if (minPrice.HasValue)
        {
            query = query.Where(t => t.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(t => t.Price <= maxPrice.Value);
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(t => t.Price),
            "price_desc" => query.OrderByDescending(t => t.Price),
            "duration" => query.OrderBy(t => t.DurationMinutes),
            _ => query.OrderByDescending(t => t.IsFeatured).ThenBy(t => t.DepartureTime)
        };

        var totalCount = await query.CountAsync();
        var transports = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = transports.Select(t => new TransportDto
        {
            TransportId = t.Id,
            Type = t.Type,
            Provider = t.Provider,
            FromCity = t.FromCity,
            ToCity = t.ToCity,
            Route = t.Route,
            DepartureTime = t.DepartureTime?.ToString(@"hh\:mm"),
            ArrivalTime = t.ArrivalTime?.ToString(@"hh\:mm"),
            DurationMinutes = t.DurationMinutes,
            Price = t.Price,
            AvailableSeats = t.AvailableSeats,
            Amenities = string.IsNullOrEmpty(t.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(t.Amenities),
            Images = string.IsNullOrEmpty(t.Images) ? null : JsonSerializer.Deserialize<List<string>>(t.Images),
            IsFeatured = t.IsFeatured
        }).ToList();

        return Ok(new ApiResponse<List<TransportDto>>
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

    /// <summary>
    /// Lấy thông tin phương tiện theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TransportDto>>> GetTransport(Guid id)
    {
        var transport = await _context.Transports
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

        if (transport == null)
        {
            return NotFound(new ApiResponse<TransportDto>
            {
                Success = false,
                Message = "Không tìm thấy phương tiện"
            });
        }

        var transportDto = new TransportDto
        {
            TransportId = transport.Id,
            Type = transport.Type,
            Provider = transport.Provider,
            FromCity = transport.FromCity,
            ToCity = transport.ToCity,
            Route = transport.Route,
            DepartureTime = transport.DepartureTime?.ToString(@"hh\:mm"),
            ArrivalTime = transport.ArrivalTime?.ToString(@"hh\:mm"),
            DurationMinutes = transport.DurationMinutes,
            Price = transport.Price,
            AvailableSeats = transport.AvailableSeats,
            Amenities = string.IsNullOrEmpty(transport.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(transport.Amenities),
            Images = string.IsNullOrEmpty(transport.Images) ? null : JsonSerializer.Deserialize<List<string>>(transport.Images),
            IsFeatured = transport.IsFeatured
        };

        return Ok(new ApiResponse<TransportDto>
        {
            Success = true,
            Data = transportDto
        });
    }

    /// <summary>
    /// Lấy danh sách các loại phương tiện
    /// </summary>
    [HttpGet("types")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetTransportTypes()
    {
        var types = await _context.Transports
            .Where(t => t.IsActive)
            .Select(t => t.Type)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        return Ok(new ApiResponse<List<string>>
        {
            Success = true,
            Data = types
        });
    }

    /// <summary>
    /// Lấy danh sách các thành phố
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetCities()
    {
        var fromCities = await _context.Transports
            .Where(t => t.IsActive)
            .Select(t => t.FromCity)
            .Distinct()
            .ToListAsync();

        var toCities = await _context.Transports
            .Where(t => t.IsActive)
            .Select(t => t.ToCity)
            .Distinct()
            .ToListAsync();

        var cities = fromCities.Union(toCities).OrderBy(c => c).ToList();

        return Ok(new ApiResponse<List<string>>
        {
            Success = true,
            Data = cities
        });
    }
}
