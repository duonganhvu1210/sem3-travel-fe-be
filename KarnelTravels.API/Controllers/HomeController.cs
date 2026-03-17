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
            // 1. Get banner data
            var banner = await GetBannerAsync();

            // 2. Get company info
            var companyInfo = await GetCompanyInfoAsync();

            // 3. Get featured tourist spots (at least 6)
            var featuredSpots = await GetFeaturedSpotsAsync(6);

            // 4. Get service categories with counts
            var serviceCategories = await GetServiceCategoriesAsync();

            // 5. Get contact info
            var contactInfo = await GetContactInfoAsync();

            // 6. Get social links
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
        // Get active promotions that can be used as banners
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
                Subtitle = activePromotion.Description ?? "Exclusive offers are waiting for you",
                ImageUrl = "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=1920&q=80",
                CtaText = "View now",
                CtaLink = $"/info/promotions/{activePromotion.Id}",
                IsActive = true
            };
        }

        // Default banner
        return new BannerDto
        {
            Id = "default-banner",
            Title = "Discover Vietnam with KarnelTravels",
            Subtitle = "Your journey starts here - Experience professional service at a reasonable price",
            ImageUrl = "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=1920&q=80",
            CtaText = "Explore now",
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
            Tagline = "Your trusted companion on every journey",
            Description = "Karnel Travels is proud to be one of Vietnam’s leading travel and tourism companies, offering professional services from transportation and accommodation to full-package tours.",
            AboutTitle = "Why Choose KarnelTravels?",
            AboutPoints = new List<string>
            {
                "More than 10 years of experience in the travel industry",
                "A professional and dedicated team of tour guides",
                "Partnerships with hundreds of premium hotels and resorts",
                "Commitment to the best prices on the market",
                "24/7 support throughout your entire journey"
            },
            AboutImage = "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=800&q=80",
            Features = new List<FeatureDto>
            {
                new() { Icon = "CheckCircle", Title = "Best Prices", Description = "Committed to offering the most competitive prices" },
                new() { Icon = "Users", Title = "24/7 Support", Description = "Friendly and dedicated support team" },
                new() { Icon = "Star", Title = "Highly Rated", Description = "More than 10,000+ five-star reviews" },
                new() { Icon = "Heart", Title = "Trusted Brand", Description = "Years of experience and customer trust" }
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
            // Fill with remaining spots if there are not enough featured ones
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
                Name = "Tourist Attractions",
                Icon = "MapPin",
                Description = "Explore famous tourist destinations",
                ItemCount = spotCount,
                Link = "/info/destinations",
                Color = "#0d9488"
            },
            new()
            {
                Id = "cat-2",
                Name = "Tours",
                Icon = "Palmtree",
                Description = "Exciting all-inclusive tour packages",
                ItemCount = tourCount,
                Link = "/info/tours",
                Color = "#0891b2"
            },
            new()
            {
                Id = "cat-3",
                Name = "Hotels",
                Icon = "Building2",
                Description = "Stay at premium hotels",
                ItemCount = hotelCount,
                Link = "/info/hotels",
                Color = "#6366f1"
            },
            new()
            {
                Id = "cat-4",
                Name = "Restaurants",
                Icon = "Utensils",
                Description = "Enjoy a wide range of culinary experiences",
                ItemCount = restaurantCount,
                Link = "/info/restaurants",
                Color = "#f59e0b"
            },
            new()
            {
                Id = "cat-5",
                Name = "Resorts",
                Icon = "Sun",
                Description = "Relax at luxury resorts",
                ItemCount = resortCount,
                Link = "/info/resorts",
                Color = "#ec4899"
            },
            new()
            {
                Id = "cat-6",
                Name = "Transportation",
                Icon = "Bus",
                Description = "Convenient transportation services",
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
            Hotline = "1900 6677",
            Phone = "+84 28 1234 5678",
            Email = "info@karneltravels.com",
            Address = "123 Nguyen Trai Street, District 1, Ho Chi Minh City",
            Website = "www.karneltravels.com",
            WorkingHours = "08:00 - 20:00 (Monday - Sunday)"
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