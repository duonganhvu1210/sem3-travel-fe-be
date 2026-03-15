using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public HomeController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<HomeDataDto>>> GetHomeData()
    {
        try
        {
            // 1. Get Banner data
            var banner = await GetBannerAsync();

            // 2. Get Company Info
            var companyInfo = await GetCompanyInfoAsync();

            // 3. Get Featured Tourist Spots (at least 6)
            var featuredSpots = await GetFeaturedSpotsAsync(6);

            // 4. Get Service Categories with counts
            var serviceCategories = await GetServiceCategoriesAsync();

            // 5. Get Contact Info
            var contactInfo = await GetContactInfoAsync();

            // 6. Get Social Links
            var socialLinks = await GetSocialLinksAsync();

            var homeData = new HomeDataDto
            {
                Banner = banner,
                CompanyInfo = companyInfo,
                FeaturedSpots = featuredSpots,
                ServiceCategories = serviceCategories,
                ContactInfo = contactInfo,
                SocialLinks = socialLinks
            };

            return Ok(new ApiResponse<HomeDataDto>
            {
                Success = true,
                Message = "Home data retrieved successfully",
                Data = homeData
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<HomeDataDto>
            {
                Success = false,
                Message = $"Error retrieving home data: {ex.Message}",
                Data = null
            });
        }
    }

    private async Task<BannerDto> GetBannerAsync()
    {
        // Get active promotions that could be used as banners
        var activePromotion = await _context.Promotions
            .Where(p => p.IsActive && p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow)
            .OrderByDescending(p => p.DiscountValue)
            .FirstOrDefaultAsync();

        if (activePromotion != null)
        {
            return new BannerDto
            {
                Id = activePromotion.Id.ToString(),
                Title = activePromotion.Title,
                Subtitle = activePromotion.Description ?? "Ưu đãi hấp dẫn đang chờ bạn",
                ImageUrl = "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=1920&q=80",
                CtaText = "Xem ngay",
                CtaLink = $"/info/promotions/{activePromotion.Id}",
                IsActive = true
            };
        }

        // Default banner
        return new BannerDto
        {
            Id = "default-banner",
            Title = "Khám phá Việt Nam cùng KarnelTravels",
            Subtitle = "Hành trình của bạn bắt đầu tại đây - Trải nghiệm dịch vụ chuyên nghiệp, giá cả hợp lý",
            ImageUrl = "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=1920&q=80",
            CtaText = "Khám phá ngay",
            CtaLink = "/search",
            IsActive = true
        };
    }

    private async Task<CompanyInfoDto> GetCompanyInfoAsync()
    {
        // Run counts sequentially to avoid DbContext concurrency issues
        var hotelCount = await _context.Hotels.CountAsync(h => h.IsActive);
        var tourCount = await _context.Tours.CountAsync(t => t.IsActive);
        var restaurantCount = await _context.Restaurants.CountAsync(r => r.IsActive);
        var resortCount = await _context.Resorts.CountAsync(r => r.IsActive);
        var transportCount = await _context.Transports.CountAsync(t => t.IsActive);

        return new CompanyInfoDto
        {
            Id = "company-info",
            CompanyName = "Karnel Travels",
            Tagline = "Đồng hành cùng bạn trên mọi nẻo đường",
            Description = "Karnel Travels tự hào là công ty du lịch và lữ hành hàng đầu Việt Nam, cung cấp các dịch vụ chuyên nghiệp từ vận chuyển, lưu trú đến tour trọn gói.",
            AboutTitle = "Tại sao chọn KarnelTravels?",
            AboutPoints = new List<string>
            {
                "Hơn 10 năm kinh nghiệm trong ngành du lịch",
                "Đội ngũ hướng dẫn viên chuyên nghiệp, tận tâm",
                "Đối tác của hàng trăm khách sạn, resort cao cấp",
                "Cam kết giá tốt nhất thị trường",
                "Hỗ trợ 24/7 trong suốt chuyến đi"
            },
            AboutImage = "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=800&q=80",
            Features = new List<FeatureDto>
            {
                new() { Icon = "CheckCircle", Title = "Giá tốt nhất", Description = "Cam kết giá tốt nhất thị trường" },
                new() { Icon = "Users", Title = "Hỗ trợ 24/7", Description = "Đội ngũ hỗ trợ nhiệt tình" },
                new() { Icon = "Star", Title = "Đánh giá cao", Description = "Hơn 10,000+ đánh giá 5 sao" },
                new() { Icon = "Heart", Title = "Uy tín", Description = "Nhiều năm kinh nghiệm" }
            }
        };
    }

    private async Task<List<TouristSpotCardDto>> GetFeaturedSpotsAsync(int count)
    {
        var spots = await _context.TouristSpots
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.Rating)
            .Take(count)
            .ToListAsync();

        if (spots.Count < count)
        {
            // Fill with remaining spots if not enough featured
            var remainingCount = count - spots.Count;
            var remainingSpots = await _context.TouristSpots
                .Where(s => s.IsActive && !spots.Any(sp => sp.Id == s.Id))
                .OrderByDescending(s => s.Rating)
                .Take(remainingCount)
                .ToListAsync();
            spots.AddRange(remainingSpots);
        }

        return spots.Select(s => TouristSpotCardDto.FromEntity(s)).ToList();
    }

    private async Task<List<ServiceCategoryDto>> GetServiceCategoriesAsync()
    {
        // Run counts sequentially to avoid DbContext concurrency issues
        var spotCount = await _context.TouristSpots.CountAsync(s => s.IsActive);
        var hotelCount = await _context.Hotels.CountAsync(h => h.IsActive);
        var restaurantCount = await _context.Restaurants.CountAsync(r => r.IsActive);
        var resortCount = await _context.Resorts.CountAsync(r => r.IsActive);
        var transportCount = await _context.Transports.CountAsync(t => t.IsActive);
        var tourCount = await _context.Tours.CountAsync(t => t.IsActive);

        return new List<ServiceCategoryDto>
        {
            new()
            {
                Id = "cat-1",
                Name = "Điểm du lịch",
                Icon = "MapPin",
                Description = "Khám phá các địa điểm du lịch nổi tiếng",
                ItemCount = spotCount,
                Link = "/info/destinations",
                Color = "#0d9488"
            },
            new()
            {
                Id = "cat-2",
                Name = "Tour du lịch",
                Icon = "Palmtree",
                Description = "Các gói tour trọn gói hấp dẫn",
                ItemCount = tourCount,
                Link = "/info/tours",
                Color = "#0891b2"
            },
            new()
            {
                Id = "cat-3",
                Name = "Khách sạn",
                Icon = "Building2",
                Description = "Lưu trú tại các khách sạn cao cấp",
                ItemCount = hotelCount,
                Link = "/info/hotels",
                Color = "#6366f1"
            },
            new()
            {
                Id = "cat-4",
                Name = "Nhà hàng",
                Icon = "Utensils",
                Description = "Trải nghiệm ẩm thực đa dạng",
                ItemCount = restaurantCount,
                Link = "/info/restaurants",
                Color = "#f59e0b"
            },
            new()
            {
                Id = "cat-5",
                Name = "Resort",
                Icon = "Sun",
                Description = "Nghỉ dưỡng tại các resort cao cấp",
                ItemCount = resortCount,
                Link = "/info/resorts",
                Color = "#ec4899"
            },
            new()
            {
                Id = "cat-6",
                Name = "Vận chuyển",
                Icon = "Bus",
                Description = "Dịch vụ vận chuyển tiện lợi",
                ItemCount = transportCount,
                Link = "/info/transports",
                Color = "#8b5cf6"
            }
        };
    }

    private async Task<ContactInfoDto> GetContactInfoAsync()
    {
        // Try to get contact info from database
        var contact = await _context.Contacts
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        return new ContactInfoDto
        {
            CompanyName = "Karnel Travels",
            Hotline = "1900 xxxx",
            Phone = "+84 28 1234 5678",
            Email = "info@karneltravels.com",
            Address = "123 Đường Nguyễn Trãi, Quận 1, TP. Hồ Chí Minh",
            Website = "www.karneltravels.com",
            WorkingHours = "08:00 - 20:00 (Thứ 2 - Chủ nhật)"
        };
    }

    private async Task<SocialLinksDto> GetSocialLinksAsync()
    {
        return new SocialLinksDto
        {
            Facebook = "https://facebook.com/karneltravels",
            Instagram = "https://instagram.com/karneltravels",
            YouTube = "https://youtube.com/karneltravels",
            Zalo = "https://zalo.me/karneltravels",
            Twitter = "https://twitter.com/karneltravels",
            TikTok = "https://tiktok.com/@karneltravels"
        };
    }
}
