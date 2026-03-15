using System.Text.Json;

namespace KarnelTravels.API.DTOs;

// ==================== HOME DATA VIEW MODELS ====================

public class HomeDataDto
{
    public BannerDto Banner { get; set; } = new();
    public CompanyInfoDto CompanyInfo { get; set; } = new();
    public List<TouristSpotCardDto> FeaturedSpots { get; set; } = new();
    public List<ServiceCategoryDto> ServiceCategories { get; set; } = new();
    public ContactInfoDto ContactInfo { get; set; } = new();
    public SocialLinksDto SocialLinks { get; set; } = new();
}

public class BannerDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string CtaText { get; set; } = "Khám phá ngay";
    public string CtaLink { get; set; } = "/search";
    public bool IsActive { get; set; } = true;
}

public class CompanyInfoDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CompanyName { get; set; } = "Karnel Travels";
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AboutTitle { get; set; } = "Về chúng tôi";
    public List<string> AboutPoints { get; set; } = new();
    public string? AboutImage { get; set; }
    public List<FeatureDto> Features { get; set; } = new();
}

public class FeatureDto
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TouristSpotCardDto
{
    public Guid SpotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? TicketPrice { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? BestTime { get; set; }
    public bool IsFeatured { get; set; }

    public static TouristSpotCardDto FromEntity(Entities.TouristSpot spot)
    {
        var images = string.IsNullOrEmpty(spot.Images) 
            ? new List<string>() 
            : JsonSerializer.Deserialize<List<string>>(spot.Images) ?? new List<string>();

        return new TouristSpotCardDto
        {
            SpotId = spot.Id,
            Name = spot.Name,
            Description = spot.Description,
            Region = spot.Region,
            Type = spot.Type,
            Address = spot.Address,
            City = spot.City,
            ImageUrl = images.FirstOrDefault() ?? "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=800&q=80",
            TicketPrice = spot.TicketPrice,
            Rating = spot.Rating,
            ReviewCount = spot.ReviewCount,
            BestTime = spot.BestTime,
            IsFeatured = spot.IsFeatured
        };
    }
}

public class ServiceCategoryDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Color { get; set; } = "#0d9488";
}

public class ContactInfoDto
{
    public string CompanyName { get; set; } = "Karnel Travels";
    public string? Hotline { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public string? WorkingHours { get; set; }
}

public class SocialLinksDto
{
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? YouTube { get; set; }
    public string? Zalo { get; set; }
    public string? Twitter { get; set; }
    public string? TikTok { get; set; }
}

// ==================== PROMOTION UPDATE DTO (SignalR) ====================

public class PromotionUpdateDto
{
    public Guid PromoId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public string UpdateType { get; set; } = "updated"; // created, updated, deleted, expired
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ==================== REQUEST MODELS ====================

public class UpdateHomeDataRequest
{
    public BannerDto? Banner { get; set; }
    public CompanyInfoDto? CompanyInfo { get; set; }
    public ContactInfoDto? ContactInfo { get; set; }
    public SocialLinksDto? SocialLinks { get; set; }
}
