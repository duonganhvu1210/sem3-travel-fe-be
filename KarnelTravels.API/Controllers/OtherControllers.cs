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
public class PromotionsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public PromotionsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PromotionDto>>>> GetPromotions()
    {
        var promotions = await _context.Promotions
            .Where(p => p.IsActive && p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow)
            .ToListAsync();

        var result = promotions.Select(p => new PromotionDto
        {
            PromoId = p.Id,
            Code = p.Code,
            Title = p.Title,
            Description = p.Description,
            DiscountType = p.DiscountType.ToString(),
            DiscountValue = p.DiscountValue,
            MinOrderAmount = p.MinOrderAmount,
            MaxDiscountAmount = p.MaxDiscountAmount,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            ApplicableTo = string.IsNullOrEmpty(p.ApplicableTo) ? null : JsonSerializer.Deserialize<List<string>>(p.ApplicableTo),
            IsActive = p.IsActive,
            ShowOnHome = p.ShowOnHome
        }).ToList();

        return Ok(new ApiResponse<List<PromotionDto>>
        {
            Success = true,
            Data = result
        });
    }

    [HttpGet("validate")]
    public async Task<ActionResult<ApiResponse<PromotionDto>>> ValidatePromoCode(
        [FromQuery] string code,
        [FromQuery] string orderType,
        [FromQuery] decimal orderAmount)
    {
        var promotion = await _context.Promotions
            .FirstOrDefaultAsync(p => p.Code == code && p.IsActive && p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow);

        if (promotion == null)
        {
            return NotFound(new ApiResponse<PromotionDto>
            {
                Success = false,
                Message = "Invalid promo code"
            });
        }

        if (promotion.MinOrderAmount.HasValue && orderAmount < promotion.MinOrderAmount.Value)
        {
            return BadRequest(new ApiResponse<PromotionDto>
            {
                Success = false,
                Message = $"Minimum order amount is {promotion.MinOrderAmount.Value:C}"
            });
        }

        return Ok(new ApiResponse<PromotionDto>
        {
            Success = true,
            Data = new PromotionDto
            {
                PromoId = promotion.Id,
                Code = promotion.Code,
                Title = promotion.Title,
                DiscountType = promotion.DiscountType.ToString(),
                DiscountValue = promotion.DiscountValue,
                MinOrderAmount = promotion.MinOrderAmount,
                MaxDiscountAmount = promotion.MaxDiscountAmount
            }
        });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public ReviewsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet("{itemType}/{itemId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetReviews(string itemType, Guid itemId)
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.Type.ToString() == itemType && r.UserId == itemId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var result = reviews.Select(r => new ReviewDto
        {
            ReviewId = r.Id,
            Rating = r.Rating,
            Title = r.Title,
            Content = r.Content,
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            Type = r.Type.ToString(),
            UserName = r.User?.FullName ?? "Anonymous",
            UserAvatar = r.User?.Avatar,
            CreatedAt = r.CreatedAt
        }).ToList();

        return Ok(new ApiResponse<List<ReviewDto>>
        {
            Success = true,
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> CreateReview([FromBody] CreateReviewRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        var review = new Review
        {
            UserId = userId,
            Rating = request.Rating,
            Title = request.Title,
            Content = request.Content,
            Images = request.Images != null ? JsonSerializer.Serialize(request.Images) : null,
            Type = Enum.Parse<ReviewType>(request.Type)
        };

        if (request.TouristSpotId.HasValue) review.TouristSpotId = request.TouristSpotId;
        if (request.HotelId.HasValue) review.HotelId = request.HotelId;
        if (request.RestaurantId.HasValue) review.RestaurantId = request.RestaurantId;
        if (request.ResortId.HasValue) review.ResortId = request.ResortId;
        if (request.TourPackageId.HasValue) review.TourPackageId = request.TourPackageId;
        if (request.BookingId.HasValue) review.BookingId = request.BookingId;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<ReviewDto>
        {
            Success = true,
            Message = "Review submitted successfully"
        });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public FavoritesController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<FavoriteDto>>>> GetFavorites()
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .ToListAsync();

        var result = favorites.Select(f => new FavoriteDto
        {
            FavoriteId = f.Id,
            ItemType = f.ItemType.ToString(),
            ItemId = GetItemId(f),
            ItemName = GetItemName(f),
            ItemImage = GetItemImage(f),
            ItemPrice = GetItemPrice(f),
            CreatedAt = f.CreatedAt
        }).ToList();

        return Ok(new ApiResponse<List<FavoriteDto>>
        {
            Success = true,
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FavoriteDto>>> AddFavorite([FromBody] CreateFavoriteRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        var existing = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemType.ToString() == request.ItemType && GetItemIdField(request.ItemType, f) == request.ItemId);

        if (existing != null)
        {
            return BadRequest(new ApiResponse<FavoriteDto>
            {
                Success = false,
                Message = "Item already in favorites"
            });
        }

        var favorite = new Favorite
        {
            UserId = userId,
            ItemType = Enum.Parse<FavoriteType>(request.ItemType)
        };

        switch (request.ItemType)
        {
            case "TouristSpot": favorite.TouristSpotId = request.ItemId; break;
            case "Hotel": favorite.HotelId = request.ItemId; break;
            case "Restaurant": favorite.RestaurantId = request.ItemId; break;
            case "Resort": favorite.ResortId = request.ItemId; break;
            case "Tour": favorite.TourPackageId = request.ItemId; break;
            case "Transport": favorite.TransportId = request.ItemId; break;
        }

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<FavoriteDto>
        {
            Success = true,
            Message = "Added to favorites"
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> RemoveFavorite(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

        if (favorite == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "Favorite not found"
            });
        }

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Removed from favorites"
        });
    }

    private Guid GetItemId(Favorite f)
    {
        return f.TouristSpotId ?? f.HotelId ?? f.RestaurantId ?? f.ResortId ?? f.TourPackageId ?? f.TransportId ?? Guid.Empty;
    }

    private string GetItemName(Favorite f)
    {
        return "Item";
    }

    private string? GetItemImage(Favorite f)
    {
        return null;
    }

    private decimal? GetItemPrice(Favorite f)
    {
        return null;
    }

    private Guid? GetItemIdField(string itemType, Favorite f)
    {
        return itemType switch
        {
            "TouristSpot" => f.TouristSpotId,
            "Hotel" => f.HotelId,
            "Restaurant" => f.RestaurantId,
            "Resort" => f.ResortId,
            "Tour" => f.TourPackageId,
            "Transport" => f.TransportId,
            _ => null
        };
    }
}
