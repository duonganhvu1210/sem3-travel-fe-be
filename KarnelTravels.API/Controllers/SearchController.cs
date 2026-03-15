using KarnelTravels.API.DTOs;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public SearchController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SearchResultDto>>> SearchAll(
        [FromQuery] string? q,
        [FromQuery] string? category,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12)
    {
        var query = _context.TouristSpots.Where(s => s.IsActive);

        if (!string.IsNullOrEmpty(q))
            query = query.Where(s => s.Name.Contains(q) || s.City.Contains(q));

        var totalCount = await query.CountAsync();
        var spots = await query
            .OrderByDescending(s => s.Rating)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SearchItemDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Type = "spot",
                ImageUrl = s.Images,
                Price = s.TicketPrice ?? 0,
                Rating = s.Rating,
                ReviewCount = s.ReviewCount,
                Location = s.City ?? s.Region
            })
            .ToListAsync();

        var result = new SearchResultDto
        {
            Items = spots,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        return Ok(new ApiResponse<SearchResultDto> { Success = true, Data = result });
    }

    [HttpGet("advanced")]
    public async Task<ActionResult<ApiResponse<AdvancedSearchResultDto>>> AdvancedSearch(
        [FromQuery] string? destination,
        [FromQuery] int? hotelStars,
        [FromQuery] string? transportType,
        [FromQuery] decimal? maxBudget)
    {
        var results = new AdvancedSearchResultDto();

        // Search for spots at destination
        if (!string.IsNullOrEmpty(destination))
        {
            var spots = await _context.TouristSpots
                .Where(s => s.IsActive && (s.Name.Contains(destination) || s.City.Contains(destination)))
                .OrderByDescending(s => s.Rating)
                .Take(5)
                .Select(s => new SearchItemDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Type = "spot",
                    ImageUrl = s.Images,
                    Price = s.TicketPrice ?? 0,
                    Rating = s.Rating,
                    ReviewCount = s.ReviewCount,
                    Location = s.City ?? s.Region
                })
                .ToListAsync();
            results.Spots = spots;
        }

        // Search for hotels
        if (hotelStars.HasValue)
        {
            var hotels = await _context.Hotels
                .Where(h => h.IsActive && h.StarRating >= hotelStars)
                .OrderByDescending(h => h.Rating)
                .Take(5)
                .ToListAsync();

            var hotelDtos = hotels.Select(h => new SearchItemDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = "hotel",
                ImageUrl = h.Images,
                Price = h.MinPrice ?? 0,
                Rating = h.Rating,
                ReviewCount = h.ReviewCount,
                Location = h.Address ?? h.City,
                Stars = h.StarRating
            }).ToList();

            if (maxBudget.HasValue)
                hotelDtos = hotelDtos.Where(h => h.Price <= maxBudget.Value).ToList();

            results.Hotels = hotelDtos;
        }

        // Search for transports
        if (!string.IsNullOrEmpty(transportType))
        {
            var transports = await _context.Transports
                .Where(t => t.IsActive && t.Type.Contains(transportType))
                .OrderByDescending(t => t.Price)
                .Take(5)
                .ToListAsync();

            var transportDtos = transports.Select(t => new SearchItemDto
            {
                Id = t.Id,
                Name = t.Provider,
                Description = t.Route,
                Type = "transport",
                ImageUrl = t.Images,
                Price = t.Price,
                Rating = 0,
                ReviewCount = 0,
                Location = t.FromCity + " - " + t.ToCity
            }).ToList();

            results.Transports = transportDtos;
        }

        // Calculate total budget
        if (maxBudget.HasValue)
        {
            decimal total = 0;
            if (results.Hotels != null)
                total += results.Hotels.Sum(h => h.Price);
            if (results.Spots != null)
                total += results.Spots.Sum(s => s.Price);
            if (results.Transports != null)
                total += results.Transports.Sum(t => t.Price);

            results.WithinBudget = total <= maxBudget.Value;
            results.TotalEstimatedPrice = total;
        }

        return Ok(new ApiResponse<AdvancedSearchResultDto> { Success = true, Data = results });
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<ApiResponse<List<SuggestionDto>>>> GetSuggestions(
        [FromQuery] string q)
    {
        if (string.IsNullOrEmpty(q) || q.Length < 2)
            return Ok(new ApiResponse<List<SuggestionDto>> { Success = true, Data = new List<SuggestionDto>() });

        var suggestions = new List<SuggestionDto>();

        // Search spots
        var spots = await _context.TouristSpots
            .Where(s => s.IsActive && s.Name.Contains(q))
            .Take(3)
            .Select(s => new SuggestionDto { Text = s.Name, Type = "spot", Subtext = s.City })
            .ToListAsync();
        suggestions.AddRange(spots);

        // Search hotels
        var hotels = await _context.Hotels
            .Where(h => h.IsActive && h.Name.Contains(q))
            .Take(2)
            .Select(h => new SuggestionDto { Text = h.Name, Type = "hotel", Subtext = h.City })
            .ToListAsync();
        suggestions.AddRange(hotels);

        // Search tours
        var tours = await _context.Tours
            .Where(t => t.IsActive && t.Name.Contains(q))
            .Take(2)
            .Select(t => new SuggestionDto { Text = t.Name, Type = "tour", Subtext = t.DurationDays + " days" })
            .ToListAsync();
        suggestions.AddRange(tours);

        return Ok(new ApiResponse<List<SuggestionDto>> { Success = true, Data = suggestions });
    }

    [HttpPost("history")]
    public async Task<ActionResult<ApiResponse<SearchHistoryDto>>> SaveSearchHistory(
        [FromBody] SaveSearchHistoryRequest request)
    {
        var history = new Entities.SearchHistory
        {
            UserId = request.UserId,
            SearchQuery = request.Query,
            Category = request.Category,
            Location = request.Location,
            SearchDate = DateTime.UtcNow
        };

        _context.SearchHistories.Add(history);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<SearchHistoryDto> { Success = true });
    }

    [HttpGet("history/{userId}")]
    public async Task<ActionResult<ApiResponse<List<SearchHistoryDto>>>> GetSearchHistory(Guid userId)
    {
        var history = await _context.SearchHistories
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.SearchDate)
            .Take(10)
            .Select(h => new SearchHistoryDto
            {
                Id = h.Id,
                SearchQuery = h.SearchQuery,
                Category = h.Category,
                SearchDate = h.SearchDate,
                Location = h.Location
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<SearchHistoryDto>> { Success = true, Data = history });
    }
}

// DTOs
public class SearchResultDto
{
    public List<SearchItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public class AdvancedSearchResultDto
{
    public List<SearchItemDto>? Spots { get; set; }
    public List<SearchItemDto>? Hotels { get; set; }
    public List<SearchItemDto>? Transports { get; set; }
    public decimal TotalEstimatedPrice { get; set; }
    public bool WithinBudget { get; set; } = true;
}

public class SearchItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? Location { get; set; }
    public int? Stars { get; set; }
}

public class SuggestionDto
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Subtext { get; set; }
}

public class SearchHistoryDto
{
    public Guid Id { get; set; }
    public string SearchQuery { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
    public string? Location { get; set; }
}

public class SaveSearchHistoryRequest
{
    public Guid UserId { get; set; }
    public string Query { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Location { get; set; }
}
