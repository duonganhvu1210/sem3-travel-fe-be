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

    // =========== Unified Endpoints for Frontend ===========
    
    /// <summary>
    /// Get hotels with pagination
    /// </summary>
    [HttpGet("hotels")]
    public async Task<ActionResult<ApiResponse<List<SearchItemDto>>>> GetHotels(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] int? stars = null)
    {
        var query = _context.Hotels.Where(h => h.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(h => h.Name.Contains(search) || (h.Address != null && h.Address.Contains(search)));

        if (stars.HasValue)
            query = query.Where(h => h.StarRating == stars.Value);

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(h => h.MinPrice),
            "price_desc" => query.OrderByDescending(h => h.MinPrice),
            "rating" => query.OrderByDescending(h => h.Rating),
            "name" => query.OrderBy(h => h.Name),
            _ => query.OrderByDescending(h => h.IsFeatured).ThenByDescending(h => h.Rating)
        };

        var totalCount = await query.CountAsync();
        var hotels = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(h => new SearchItemDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = "hotel",
                ImageUrl = h.Images,
                Price = h.MinPrice ?? 0,
                Rating = h.Rating,
                ReviewCount = h.ReviewCount,
                Location = h.City,
                AdditionalInfo = h.StarRating.ToString()
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<SearchItemDto>> 
        { 
            Success = true, 
            Data = hotels,
            Pagination = new PaginationInfo { PageIndex = pageIndex, PageSize = pageSize, TotalCount = totalCount }
        });
    }

    /// <summary>
    /// Get tours with pagination
    /// </summary>
    [HttpGet("tours")]
    public async Task<ActionResult<ApiResponse<List<SearchItemDto>>>> GetTours(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] string? destination = null)
    {
        var query = _context.TourPackages.Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Name.Contains(search) || t.Destination.Contains(search));

        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(t => t.Destination.Contains(destination));

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(t => t.Price),
            "price_desc" => query.OrderByDescending(t => t.Price),
            "rating" => query.OrderByDescending(t => t.Rating),
            "name" => query.OrderBy(t => t.Name),
            _ => query.OrderByDescending(t => t.IsFeatured).ThenByDescending(t => t.Rating)
        };

        var totalCount = await query.CountAsync();
        var tours = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new SearchItemDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Type = "tour",
                ImageUrl = t.Images,
                Price = t.DiscountPrice ?? t.Price,
                Rating = t.Rating,
                ReviewCount = t.ReviewCount,
                Location = t.Destination,
                AdditionalInfo = t.DurationDays.ToString()
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<SearchItemDto>> 
        { 
            Success = true, 
            Data = tours,
            Pagination = new PaginationInfo { PageIndex = pageIndex, PageSize = pageSize, TotalCount = totalCount }
        });
    }

    /// <summary>
    /// Get restaurants with pagination
    /// </summary>
    [HttpGet("restaurants")]
    public async Task<ActionResult<ApiResponse<List<SearchItemDto>>>> GetRestaurants(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] string? city = null)
    {
        var query = _context.Restaurants.Where(r => r.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Name.Contains(search) || (r.Description != null && r.Description.Contains(search)));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(r => r.City == city);

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(r => r.PriceLevel),
            "price_desc" => query.OrderByDescending(r => r.PriceLevel),
            "rating" => query.OrderByDescending(r => r.Rating),
            "name" => query.OrderBy(r => r.Name),
            _ => query.OrderByDescending(r => r.IsFeatured).ThenByDescending(r => r.Rating)
        };

        var totalCount = await query.CountAsync();
        var restaurants = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new SearchItemDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Type = "restaurant",
                ImageUrl = r.Images,
                Price = 0,
                Rating = r.Rating,
                ReviewCount = r.ReviewCount,
                Location = r.City,
                AdditionalInfo = r.CuisineType
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<SearchItemDto>> 
        { 
            Success = true, 
            Data = restaurants,
            Pagination = new PaginationInfo { PageIndex = pageIndex, PageSize = pageSize, TotalCount = totalCount }
        });
    }

    /// <summary>
    /// Get transports with pagination
    /// </summary>
    [HttpGet("transports")]
    public async Task<ActionResult<ApiResponse<List<SearchItemDto>>>> GetTransports(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] string? fromCity = null,
        [FromQuery] string? toCity = null)
    {
        var query = _context.Transports.Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Provider.Contains(search) || t.Route.Contains(search));

        if (!string.IsNullOrWhiteSpace(fromCity))
            query = query.Where(t => t.FromCity == fromCity);

        if (!string.IsNullOrWhiteSpace(toCity))
            query = query.Where(t => t.ToCity == toCity);

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(t => t.Price),
            "price_desc" => query.OrderByDescending(t => t.Price),
            _ => query.OrderByDescending(t => t.IsFeatured)
        };

        var totalCount = await query.CountAsync();
        var transports = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new SearchItemDto
            {
                Id = t.Id,
                Name = t.Provider,
                Description = t.Route,
                Type = "transport",
                ImageUrl = t.Images,
                Price = t.Price,
                Rating = 0,
                ReviewCount = 0,
                Location = $"{t.FromCity} → {t.ToCity}",
                AdditionalInfo = t.Type
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<SearchItemDto>> 
        { 
            Success = true, 
            Data = transports,
            Pagination = new PaginationInfo { PageIndex = pageIndex, PageSize = pageSize, TotalCount = totalCount }
        });
    }

    /// <summary>
    /// Get resorts with pagination
    /// </summary>
    [HttpGet("resorts")]
    public async Task<ActionResult<ApiResponse<List<SearchItemDto>>>> GetResorts(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] int? stars = null)
    {
        var query = _context.Resorts.Where(r => r.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Name.Contains(search) || (r.Address != null && r.Address.Contains(search)));

        if (stars.HasValue)
            query = query.Where(r => r.StarRating == stars.Value);

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(r => r.MinPrice),
            "price_desc" => query.OrderByDescending(r => r.MinPrice),
            "rating" => query.OrderByDescending(r => r.Rating),
            "name" => query.OrderBy(r => r.Name),
            _ => query.OrderByDescending(r => r.IsFeatured).ThenByDescending(r => r.Rating)
        };

        var totalCount = await query.CountAsync();
        var resorts = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new SearchItemDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Type = "resort",
                ImageUrl = r.Images,
                Price = r.MinPrice ?? 0,
                Rating = r.Rating,
                ReviewCount = r.ReviewCount,
                Location = r.City,
                AdditionalInfo = r.StarRating.ToString()
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<SearchItemDto>> 
        { 
            Success = true, 
            Data = resorts,
            Pagination = new PaginationInfo { PageIndex = pageIndex, PageSize = pageSize, TotalCount = totalCount }
        });
    }

    /// <summary>
    /// Get popular search terms
    /// </summary>
    [HttpGet("popular")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetPopularSearches()
    {
        var popular = new List<string>
        {
            "Đà Nẵng", "Phú Quốc", "Nha Trang", "Hà Nội", "TP. Hồ Chí Minh",
            "Khách sạn", "Resort", "Tour du lịch", "Nhà hàng", "Biển"
        };
        
        return Ok(new ApiResponse<List<string>> { Success = true, Data = popular });
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
    public string? AdditionalInfo { get; set; }
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
