using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransportsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public TransportsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    // F077: Search by Route - Tìm kiếm theo tuyến đường
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<TransportDto>>>> SearchByRoute(
        [FromQuery] string? route,
        [FromQuery] string? fromCity,
        [FromQuery] string? toCity,
        [FromQuery] string? type,
        [FromQuery] string? provider,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? departureTime,
        [FromQuery] double? minDuration,
        [FromQuery] double? maxDuration,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder = "ASC",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12)
    {
        var query = _context.Transports.Where(t => t.IsActive).AsQueryable();

        // F077: Search by route keyword
        if (!string.IsNullOrEmpty(route))
            query = query.Where(t => (t.Route != null && t.Route.Contains(route)) || 
                                      t.FromCity.Contains(route) || t.ToCity.Contains(route));

        // F078-F081: Filters
        if (!string.IsNullOrEmpty(fromCity))
            query = query.Where(t => t.FromCity == fromCity);

        if (!string.IsNullOrEmpty(toCity))
            query = query.Where(t => t.ToCity == toCity);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(t => t.Type == type);

        if (!string.IsNullOrEmpty(provider))
            query = query.Where(t => t.Provider.Contains(provider));

        if (minPrice.HasValue)
            query = query.Where(t => t.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(t => t.Price <= maxPrice.Value);

        // F075: Filter by departure time
        if (!string.IsNullOrEmpty(departureTime))
        {
            if (TimeSpan.TryParse(departureTime, out var time))
            {
                var hour = time.Hours;
                query = query.Where(t => t.DepartureTime.HasValue && 
                                        t.DepartureTime.Value.Hours >= hour &&
                                        t.DepartureTime.Value.Hours < hour + 2);
            }
        }

        // Duration filter
        if (minDuration.HasValue)
            query = query.Where(t => t.DurationMinutes >= minDuration.Value * 60);

        if (maxDuration.HasValue)
            query = query.Where(t => t.DurationMinutes <= maxDuration.Value * 60);

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "price" => sortOrder == "DESC" ? query.OrderByDescending(t => t.Price) : query.OrderBy(t => t.Price),
            "duration" => sortOrder == "DESC" ? query.OrderByDescending(t => t.DurationMinutes) : query.OrderBy(t => t.DurationMinutes),
            "departure" => sortOrder == "DESC" ? query.OrderByDescending(t => t.DepartureTime) : query.OrderBy(t => t.DepartureTime),
            "provider" => sortOrder == "DESC" ? query.OrderByDescending(t => t.Provider) : query.OrderBy(t => t.Provider),
            _ => query.OrderBy(t => t.DepartureTime)
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = items.Select(t => new TransportDto
        {
            TransportId = t.Id,
            Type = t.Type,
            Provider = t.Provider,
            FromCity = t.FromCity,
            ToCity = t.ToCity,
            Route = t.Route,
            DepartureTime = t.DepartureTime.HasValue ? t.DepartureTime.Value.ToString(@"hh\:mm") : null,
            ArrivalTime = t.ArrivalTime.HasValue ? t.ArrivalTime.Value.ToString(@"hh\:mm") : null,
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

    // F068-F072: GET all transports with basic filters
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TransportDto>>>> GetTransports(
        [FromQuery] string? type,
        [FromQuery] string? fromCity,
        [FromQuery] string? toCity,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Transports.Where(t => t.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(type))
            query = query.Where(t => t.Type == type);

        if (!string.IsNullOrEmpty(fromCity))
            query = query.Where(t => t.FromCity == fromCity);

        if (!string.IsNullOrEmpty(toCity))
            query = query.Where(t => t.ToCity == toCity);

        if (minPrice.HasValue)
            query = query.Where(t => t.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(t => t.Price <= maxPrice.Value);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = items.Select(t => new TransportDto
        {
            TransportId = t.Id,
            Type = t.Type,
            Provider = t.Provider,
            FromCity = t.FromCity,
            ToCity = t.ToCity,
            Route = t.Route,
            DepartureTime = t.DepartureTime.HasValue ? t.DepartureTime.Value.ToString(@"hh\:mm") : null,
            ArrivalTime = t.ArrivalTime.HasValue ? t.ArrivalTime.Value.ToString(@"hh\:mm") : null,
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

    // F082: Get Featured transports
    [HttpGet("featured")]
    public async Task<ActionResult<ApiResponse<List<TransportDto>>>> GetFeaturedTransports(
        [FromQuery] int pageSize = 6)
    {
        var items = await _context.Transports
            .Where(t => t.IsActive && t.IsFeatured)
            .OrderByDescending(t => t.Price)
            .Take(pageSize)
            .ToListAsync();

        var result = items.Select(t => new TransportDto
        {
            TransportId = t.Id,
            Type = t.Type,
            Provider = t.Provider,
            FromCity = t.FromCity,
            ToCity = t.ToCity,
            Route = t.Route,
            DepartureTime = t.DepartureTime.HasValue ? t.DepartureTime.Value.ToString(@"hh\:mm") : null,
            ArrivalTime = t.ArrivalTime.HasValue ? t.ArrivalTime.Value.ToString(@"hh\:mm") : null,
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
            Data = result
        });
    }

    // F075: Get average travel time statistics
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<TransportStatisticsDto>>> GetTransportStatistics()
    {
        var transports = await _context.Transports.Where(t => t.IsActive).ToListAsync();

        var stats = new TransportStatisticsDto
        {
            TotalTransports = transports.Count,
            AverageDurationMinutes = transports.Any() ? (int)transports.Average(t => t.DurationMinutes) : 0,
            AveragePrice = transports.Any() ? transports.Average(t => t.Price) : 0,
            MinPrice = transports.Any() ? transports.Min(t => t.Price) : 0,
            MaxPrice = transports.Any() ? transports.Max(t => t.Price) : 0,
            AvailableTypes = transports.Select(t => t.Type).Distinct().ToList(),
            AvailableProviders = transports.Select(t => t.Provider).Distinct().ToList(),
            AvailableRoutes = transports.Select(t => t.FromCity + " - " + t.ToCity).Distinct().ToList(),
            AverageDurationByType = transports
                .GroupBy(t => t.Type)
                .ToDictionary(g => g.Key, g => (int)g.Average(t => t.DurationMinutes))
        };

        return Ok(new ApiResponse<TransportStatisticsDto>
        {
            Success = true,
            Data = stats
        });
    }

    // Get transport by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TransportDto>>> GetTransport(Guid id)
    {
        var transport = await _context.Transports.FindAsync(id);

        if (transport == null || !transport.IsActive)
        {
            return NotFound(new ApiResponse<TransportDto>
            {
                Success = false,
                Message = "Transport not found"
            });
        }

        return Ok(new ApiResponse<TransportDto>
        {
            Success = true,
            Data = new TransportDto
            {
                TransportId = transport.Id,
                Type = transport.Type,
                Provider = transport.Provider,
                FromCity = transport.FromCity,
                ToCity = transport.ToCity,
                Route = transport.Route,
                DepartureTime = transport.DepartureTime.HasValue ? transport.DepartureTime.Value.ToString(@"hh\:mm") : null,
                ArrivalTime = transport.ArrivalTime.HasValue ? transport.ArrivalTime.Value.ToString(@"hh\:mm") : null,
                DurationMinutes = transport.DurationMinutes,
                Price = transport.Price,
                AvailableSeats = transport.AvailableSeats,
                Amenities = string.IsNullOrEmpty(transport.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(transport.Amenities),
                Images = string.IsNullOrEmpty(transport.Images) ? null : JsonSerializer.Deserialize<List<string>>(transport.Images),
                IsFeatured = transport.IsFeatured
            }
        });
    }

    // F074: Compare multiple transports
    [HttpPost("compare")]
    public async Task<ActionResult<ApiResponse<List<TransportDto>>>> CompareTransports(
        [FromBody] List<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest(new ApiResponse<List<TransportDto>>
            {
                Success = false,
                Message = "No transport IDs provided"
            });

        var transports = await _context.Transports
            .Where(t => t.IsActive && ids.Contains(t.Id))
            .ToListAsync();

        var result = transports.Select(t => new TransportDto
        {
            TransportId = t.Id,
            Type = t.Type,
            Provider = t.Provider,
            FromCity = t.FromCity,
            ToCity = t.ToCity,
            Route = t.Route,
            DepartureTime = t.DepartureTime.HasValue ? t.DepartureTime.Value.ToString(@"hh\:mm") : null,
            ArrivalTime = t.ArrivalTime.HasValue ? t.ArrivalTime.Value.ToString(@"hh\:mm") : null,
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
            Data = result
        });
    }

    // Get filter options (for sidebar)
    [HttpGet("filters")]
    public async Task<ActionResult<ApiResponse<TransportFilterOptionsDto>>> GetFilterOptions()
    {
        var transports = await _context.Transports.Where(t => t.IsActive).ToListAsync();

        var options = new TransportFilterOptionsDto
        {
            Types = transports.Select(t => t.Type).Distinct().OrderBy(t => t).ToList(),
            Providers = transports.Select(t => t.Provider).Distinct().OrderBy(p => p).ToList(),
            Cities = transports
                .SelectMany(t => new[] { t.FromCity, t.ToCity })
                .Distinct()
                .OrderBy(c => c)
                .ToList(),
            MinPrice = transports.Any() ? transports.Min(t => t.Price) : 0,
            MaxPrice = transports.Any() ? transports.Max(t => t.Price) : 0,
            MinDuration = transports.Any() ? transports.Min(t => t.DurationMinutes) : 0,
            MaxDuration = transports.Any() ? transports.Max(t => t.DurationMinutes) : 0
        };

        return Ok(new ApiResponse<TransportFilterOptionsDto>
        {
            Success = true,
            Data = options
        });
    }
}
