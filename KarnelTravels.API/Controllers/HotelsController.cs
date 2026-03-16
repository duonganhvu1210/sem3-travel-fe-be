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
public class HotelsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public HotelsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// L?y danh s?ch kh?ch s?n v?i ph?n trang v? l?c
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HotelDto>>>> GetHotels(
        [FromQuery] string? search = "",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortBy = "",
        [FromQuery] string? city = null,
        [FromQuery] int? starRating = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        var query = _context.Hotels
            .Where(h => !h.IsDeleted && h.IsActive)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h => 
                h.Name.Contains(search) ||
                (h.Description != null && h.Description.Contains(search)) ||
                h.City.Contains(search));
        }

        // City filter
        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(h => h.City == city);
        }

        // Star rating filter
        if (starRating.HasValue)
        {
            query = query.Where(h => h.StarRating == starRating.Value);
        }

        // Price filters
        if (minPrice.HasValue)
        {
            query = query.Where(h => h.MinPrice >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(h => h.MaxPrice <= maxPrice.Value);
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(h => h.MinPrice),
            "price_desc" => query.OrderByDescending(h => h.MaxPrice),
            "rating" => query.OrderByDescending(h => h.Rating),
            "name" => query.OrderBy(h => h.Name),
            _ => query.OrderByDescending(h => h.IsFeatured).ThenByDescending(h => h.Rating)
        };

        var totalCount = await query.CountAsync();
        var hotels = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = hotels.Select(h => new HotelDto
        {
            HotelId = h.Id,
            Name = h.Name,
            Description = h.Description,
            Address = h.Address,
            City = h.City,
            StarRating = h.StarRating,
            ContactPhone = h.ContactPhone,
            ContactEmail = h.ContactEmail,
            Latitude = h.Latitude,
            Longitude = h.Longitude,
            Images = string.IsNullOrEmpty(h.Images) ? null : JsonSerializer.Deserialize<List<string>>(h.Images),
            MinPrice = h.MinPrice,
            MaxPrice = h.MaxPrice,
            Amenities = string.IsNullOrEmpty(h.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(h.Amenities),
            Rating = h.Rating,
            ReviewCount = h.ReviewCount,
            IsFeatured = h.IsFeatured
        }).ToList();

        return Ok(new ApiResponse<List<HotelDto>>
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
    /// L?y th?ng tin kh?ch s?n theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<HotelDto>>> GetHotel(Guid id)
    {
        var hotel = await _context.Hotels
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted && h.IsActive);

        if (hotel == null)
        {
            return NotFound(new ApiResponse<HotelDto>
            {
                Success = false,
                Message = "Kh?ng t?m th?y kh?ch s?n"
            });
        }

        // Get rooms
        var rooms = await _context.HotelRooms
            .Where(r => r.HotelId == id && !r.IsDeleted && r.IsActive)
            .ToListAsync();

        var hotelDto = new HotelDto
        {
            HotelId = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            City = hotel.City,
            StarRating = hotel.StarRating,
            ContactPhone = hotel.ContactPhone,
            ContactEmail = hotel.ContactEmail,
            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
            Images = string.IsNullOrEmpty(hotel.Images) ? null : JsonSerializer.Deserialize<List<string>>(hotel.Images),
            MinPrice = hotel.MinPrice,
            MaxPrice = hotel.MaxPrice,
            Amenities = string.IsNullOrEmpty(hotel.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(hotel.Amenities),
            Rating = hotel.Rating,
            ReviewCount = hotel.ReviewCount,
            IsFeatured = hotel.IsFeatured
        };

        return Ok(new ApiResponse<HotelDto>
        {
            Success = true,
            Data = hotelDto
        });
    }

    /// <summary>
    /// L?y danh s?ch th?nh ph? c? kh?ch s?n
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetCities()
    {
        var cities = await _context.Hotels
            .Where(h => !h.IsDeleted && h.IsActive)
            .Select(h => h.City)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return Ok(new ApiResponse<List<string>>
        {
            Success = true,
            Data = cities
        });
    }

    /// <summary>
    /// L?y danh s�ch ph�ng c?a kh�ch s?n
    /// </summary>
    [HttpGet("{id}/rooms")]
    public async Task<ActionResult<ApiResponse<List<HotelRoomDto>>>> GetHotelRooms(Guid id)
    {
        var hotel = await _context.Hotels
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted && h.IsActive);

        if (hotel == null)
        {
            return NotFound(new ApiResponse<List<HotelRoomDto>>
            {
                Success = false,
                Message = "Kh�ng t�m th?y kh�ch s?n"
            });
        }

        var rooms = await _context.HotelRooms
            .Where(r => r.HotelId == id && !r.IsDeleted && r.IsActive)
            .ToListAsync();

        var result = rooms.Select(r => new HotelRoomDto
        {
            RoomId = r.Id,
            RoomType = r.RoomType,
            Description = r.Description,
            MaxOccupancy = r.MaxOccupancy,
            PricePerNight = r.PricePerNight,
            BedType = r.BedType,
            RoomAmenities = string.IsNullOrEmpty(r.RoomAmenities) ? null : JsonSerializer.Deserialize<List<string>>(r.RoomAmenities),
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            TotalRooms = r.TotalRooms,
            AvailableRooms = r.AvailableRooms,
            IsAvailable = r.AvailableRooms > 0
        }).ToList();

        return Ok(new ApiResponse<List<HotelRoomDto>>
        {
            Success = true,
            Data = result
        });
    }
}
