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

    // F084-F087: Search and filter hotels
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

        // F084: Search by name or city
        if (!string.IsNullOrEmpty(search))
            query = query.Where(h => h.Name.Contains(search) || h.City.Contains(search));

        // F085: Filter by city
        if (!string.IsNullOrEmpty(city))
            query = query.Where(h => h.City == city);

        // F086: Filter by star rating
        if (starRating.HasValue)
            query = query.Where(h => h.StarRating >= starRating.Value);

        // F086: Filter by price range
        if (minPrice.HasValue)
            query = query.Where(h => h.MinPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(h => h.MaxPrice <= maxPrice.Value);

        // F086: Filter by rating
        if (rating.HasValue)
            query = query.Where(h => h.Rating >= rating.Value);

        // F087: Filter by amenities (contains all specified amenities)
        if (!string.IsNullOrEmpty(amenities))
        {
            var amenityList = amenities.Split(',').Select(a => a.Trim().ToLower()).ToList();
            query = query.Where(h => h.Amenities != null && amenityList.All(a => 
                EF.Functions.Like(h.Amenities.ToLower(), $"%\"{a}\"%") ||
                EF.Functions.Like(h.Amenities.ToLower(), $"%{a}%")));
        }

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

    // F082: Get featured hotels
    [HttpGet("featured")]
    public async Task<ActionResult<ApiResponse<List<HotelDto>>>> GetFeaturedHotels(
        [FromQuery] int pageSize = 6)
    {
        var items = await _context.Hotels
            .Where(h => h.IsActive && h.IsFeatured)
            .OrderByDescending(h => h.Rating)
            .Take(pageSize)
            .ToListAsync();

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
            Data = result
        });
    }

    // Get filter options for sidebar
    [HttpGet("filters")]
    public async Task<ActionResult<ApiResponse<HotelFilterOptionsDto>>> GetFilterOptions()
    {
        var hotels = await _context.Hotels.Where(h => h.IsActive).ToListAsync();

        var options = new HotelFilterOptionsDto
        {
            Cities = hotels.Select(h => h.City).Distinct().OrderBy(c => c).ToList(),
            StarRatings = new List<int> { 5, 4, 3, 2, 1 },
            MinPrice = hotels.Any() ? hotels.Min(h => h.MinPrice) : 0,
            MaxPrice = hotels.Any() ? hotels.Max(h => h.MaxPrice) : 0,
            Amenities = hotels
                .Where(h => h.Amenities != null)
                .SelectMany(h => JsonSerializer.Deserialize<List<string>>(h.Amenities) ?? new List<string>())
                .Distinct()
                .OrderBy(a => a)
                .ToList()
        };

        return Ok(new ApiResponse<HotelFilterOptionsDto>
        {
            Success = true,
            Data = options
        });
    }

    // F088-F092: Get hotel detail with rooms, gallery, reviews
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<HotelDetailDto>>> GetHotel(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);

        if (hotel == null || !hotel.IsActive)
        {
            return NotFound(new ApiResponse<HotelDetailDto>
            {
                Success = false,
                Message = "Hotel not found"
            });
        }

        // Get rooms
        var rooms = await _context.HotelRooms
            .Where(r => r.HotelId == id && r.IsActive)
            .Select(r => new HotelRoomDto
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
            })
            .ToListAsync();

        // F089: Get hotel images
        var images = string.IsNullOrEmpty(hotel.Images) 
            ? new List<string>() 
            : JsonSerializer.Deserialize<List<string>>(hotel.Images);

        // F092: Get reviews
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.HotelId == id && r.IsActive)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .Select(r => new HotelReviewDto
            {
                ReviewId = r.Id,
                UserName = r.User != null ? r.User.FullName : "Anonymous",
                Rating = r.Rating,
                Comment = r.Content,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        var result = new HotelDetailDto
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
            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
            CancellationPolicy = hotel.CancellationPolicy,
            Images = images,
            MinPrice = hotel.MinPrice,
            MaxPrice = hotel.MaxPrice,
            Amenities = string.IsNullOrEmpty(hotel.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(hotel.Amenities),
            Rating = hotel.Rating,
            ReviewCount = hotel.ReviewCount,
            IsFeatured = hotel.IsFeatured,
            Rooms = rooms,
            Reviews = reviews
        };

        return Ok(new ApiResponse<HotelDetailDto>
        {
            Success = true,
            Data = result
        });
    }

    // F090-F091: Get hotel rooms with availability
    [HttpGet("{id}/rooms")]
    public async Task<ActionResult<ApiResponse<List<HotelRoomDto>>>> GetHotelRooms(
        Guid id,
        [FromQuery] DateTime? checkIn,
        [FromQuery] DateTime? checkOut)
    {
        var rooms = await _context.HotelRooms
            .Where(r => r.HotelId == id && r.IsActive)
            .ToListAsync();

        // F091: Check availability based on dates
        var result = rooms.Select(r => 
        {
            int availableRooms = r.AvailableRooms;
            
            // If check-in/check-out dates provided, check bookings
            if (checkIn.HasValue && checkOut.HasValue)
            {
                var bookings = _context.Bookings
                    .Where(b => b.HotelId == id && 
                               b.RoomId == r.Id &&
                               b.Status != "Cancelled" &&
                               b.CheckInDate <= checkOut.Value &&
                               b.CheckOutDate >= checkIn.Value)
                    .Count();
                
                availableRooms = Math.Max(0, r.AvailableRooms - bookings);
            }

            return new HotelRoomDto
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
                AvailableRooms = availableRooms,
                IsAvailable = availableRooms > 0
            };
        }).ToList();

        return Ok(new ApiResponse<List<HotelRoomDto>>
        {
            Success = true,
            Data = result
        });
    }

    // F092: Get hotel reviews
    [HttpGet("{id}/reviews")]
    public async Task<ActionResult<ApiResponse<List<HotelReviewDto>>>> GetHotelReviews(
        Guid id,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Reviews
            .Where(r => r.HotelId == id && r.IsActive);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new HotelReviewDto
            {
                ReviewId = r.Id,
                UserName = r.UserName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<HotelReviewDto>>
        {
            Success = true,
            Data = items,
            Pagination = new PaginationInfo
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        });
    }

    // F074: Compare multiple hotels
    [HttpPost("compare")]
    public async Task<ActionResult<ApiResponse<List<HotelDto>>>> CompareHotels(
        [FromBody] List<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest(new ApiResponse<List<HotelDto>>
            {
                Success = false,
                Message = "No hotel IDs provided"
            });

        var hotels = await _context.Hotels
            .Where(h => h.IsActive && ids.Contains(h.Id))
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
            Data = result
        });
    }
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
            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
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
