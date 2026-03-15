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
public class HotelsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public HotelsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HotelDto>>>> GetHotels(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] int? starRating,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] double? rating,
        [FromQuery] string? amenities,
        [FromQuery] bool? hasDiscount,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder = "ASC",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Hotels.Where(h => h.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(h => h.Name.Contains(search) || h.City.Contains(search));

        if (!string.IsNullOrEmpty(city))
            query = query.Where(h => h.City == city);

        if (starRating.HasValue)
            query = query.Where(h => h.StarRating >= starRating.Value);

        if (minPrice.HasValue)
            query = query.Where(h => h.MinPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(h => h.MaxPrice <= maxPrice.Value);

        if (rating.HasValue)
            query = query.Where(h => h.Rating >= rating.Value);

        // Sort
        query = sortBy?.ToLower() switch
        {
            "name" => sortOrder == "DESC" ? query.OrderByDescending(h => h.Name) : query.OrderBy(h => h.Name),
            "rating" => sortOrder == "DESC" ? query.OrderByDescending(h => h.Rating) : query.OrderBy(h => h.Rating),
            "price-asc" => query.OrderBy(h => h.MinPrice),
            "price-desc" => query.OrderByDescending(h => h.MinPrice),
            _ => query.OrderByDescending(h => h.Rating)
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = items.Select(h => new HotelDto
        {
            HotelId = h.Id,
            Name = h.Name,
            Description = h.Description,
            Address = h.Address,
            City = h.City,
            StarRating = h.StarRating,
            ContactPhone = h.ContactPhone,
            ContactEmail = h.ContactEmail,
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

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<HotelDto>>> GetHotel(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);

        if (hotel == null || !hotel.IsActive)
        {
            return NotFound(new ApiResponse<HotelDto>
            {
                Success = false,
                Message = "Hotel not found"
            });
        }

        return Ok(new ApiResponse<HotelDto>
        {
            Success = true,
            Data = new HotelDto
            {
                HotelId = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                StarRating = hotel.StarRating,
                ContactPhone = hotel.ContactPhone,
                ContactEmail = hotel.ContactEmail,
                Images = string.IsNullOrEmpty(hotel.Images) ? null : JsonSerializer.Deserialize<List<string>>(hotel.Images),
                MinPrice = hotel.MinPrice,
                MaxPrice = hotel.MaxPrice,
                Amenities = string.IsNullOrEmpty(hotel.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(hotel.Amenities),
                Rating = hotel.Rating,
                ReviewCount = hotel.ReviewCount,
                IsFeatured = hotel.IsFeatured
            }
        });
    }

    [HttpGet("{id}/rooms")]
    public async Task<ActionResult<ApiResponse<List<HotelRoomDto>>>> GetHotelRooms(Guid id)
    {
        var rooms = await _context.HotelRooms.Where(r => r.HotelId == id && r.IsActive).ToListAsync();

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
            AvailableRooms = r.AvailableRooms
        }).ToList();

        return Ok(new ApiResponse<List<HotelRoomDto>>
        {
            Success = true,
            Data = result
        });
    }
}
