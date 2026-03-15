using KarnelTravels.API.Data;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Services;

public interface IResortService
{
    // Resort CRUD
    Task<List<ResortDto>> GetAllAsync();
    Task<ResortDto?> GetByIdAsync(Guid id);
    Task<ResortDto> CreateAsync(CreateResortRequest request);
    Task<ResortDto?> UpdateAsync(Guid id, UpdateResortRequest request);
    Task<bool> DeleteAsync(Guid id);
    
    // Room Management
    Task<List<ResortRoomDto>> GetRoomsAsync(Guid resortId);
    Task<ResortRoomDto?> CreateRoomAsync(Guid resortId, CreateResortRoomRequest request);
    Task<ResortRoomDto?> UpdateRoomAsync(Guid resortId, Guid roomId, UpdateResortRoomRequest request);
    Task<bool> DeleteRoomAsync(Guid resortId, Guid roomId);
    
    // Room Availability & Pricing
    Task<List<RoomAvailabilityDto>> GetRoomAvailabilityAsync(Guid resortId, Guid roomId, DateTime startDate, int days);
    Task<bool> UpdateRoomAvailabilityAsync(Guid resortId, Guid roomId, UpdateRoomAvailabilityRequest request);
    Task<bool> BulkUpdateRoomAvailabilityAsync(Guid resortId, Guid roomId, BulkUpdateRoomAvailabilityRequest request);
    Task<List<RoomPricingDto>> GetRoomPricingAsync(Guid resortId, Guid roomId, DateTime startDate, int days);
    
    // Resort Services (Activities)
    Task<List<ResortServiceDto>> GetServicesAsync(Guid resortId);
    Task<ResortServiceDto> CreateServiceAsync(Guid resortId, CreateResortServiceRequest request);
    Task<ResortServiceDto?> UpdateServiceAsync(Guid resortId, Guid serviceId, UpdateResortServiceRequest request);
    Task<bool> DeleteServiceAsync(Guid resortId, Guid serviceId);
    
    // Combo Packages
    Task<List<ComboPackageDto>> GetPackagesAsync(Guid resortId);
    Task<ComboPackageDto> CreatePackageAsync(Guid resortId, CreateComboPackageRequest request);
    Task<ComboPackageDto?> UpdatePackageAsync(Guid resortId, Guid packageId, UpdateComboPackageRequest request);
    Task<bool> DeletePackageAsync(Guid resortId, Guid packageId);
    
    // Search & Filter
    Task<List<ResortDto>> SearchAsync(string? searchTerm, ResortType? resortType, List<ResortAmenity>? amenities, string? city);
}

public class ResortService : IResortService
{
    private readonly KarnelTravelsDbContext _context;

    public ResortService(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ResortDto>> GetAllAsync()
    {
        var resorts = await _context.Resorts
            .Include(r => r.Rooms)
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.IsFeatured)
            .ThenByDescending(r => r.Rating)
            .ToListAsync();

        return resorts.Select(MapToDto).ToList();
    }

    public async Task<ResortDto?> GetByIdAsync(Guid id)
    {
        var resort = await _context.Resorts
            .Include(r => r.Rooms)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        return resort == null ? null : MapToDto(resort);
    }

    public async Task<ResortDto> CreateAsync(CreateResortRequest request)
    {
        var resort = new Resort
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            LocationType = request.LocationType.ToString(),
            StarRating = request.StarRating,
            Images = request.Images != null ? System.Text.Json.JsonSerializer.Serialize(request.Images) : null,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            Amenities = request.Amenities != null ? System.Text.Json.JsonSerializer.Serialize(request.Amenities) : null,
            IsFeatured = request.IsFeatured,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Resorts.Add(resort);
        await _context.SaveChangesAsync();

        return MapToDto(resort);
    }

    public async Task<ResortDto?> UpdateAsync(Guid id, UpdateResortRequest request)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (resort == null) return null;

        if (request.Name != null) resort.Name = request.Name;
        if (request.Description != null) resort.Description = request.Description;
        if (request.Address != null) resort.Address = request.Address;
        if (request.City != null) resort.City = request.City;
        if (request.LocationType.HasValue) resort.LocationType = request.LocationType.Value.ToString();
        if (request.StarRating.HasValue) resort.StarRating = request.StarRating.Value;
        if (request.Images != null) resort.Images = System.Text.Json.JsonSerializer.Serialize(request.Images);
        if (request.MinPrice.HasValue) resort.MinPrice = request.MinPrice;
        if (request.MaxPrice.HasValue) resort.MaxPrice = request.MaxPrice;
        if (request.Amenities != null) resort.Amenities = System.Text.Json.JsonSerializer.Serialize(request.Amenities);
        if (request.IsFeatured.HasValue) resort.IsFeatured = request.IsFeatured.Value;

        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(resort);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (resort == null) return false;

        // Check for active bookings
        var hasActiveBookings = await _context.Bookings
            .AnyAsync(b => b.ResortId == id && 
                          !b.IsDeleted && 
                          b.Status != BookingStatus.Cancelled &&
                          b.EndDate >= DateTime.Now);

        if (hasActiveBookings)
        {
            return false;
        }

        resort.IsDeleted = true;
        resort.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ResortRoomDto>> GetRoomsAsync(Guid resortId)
    {
        var resort = await _context.Resorts
            .Include(r => r.Rooms)
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort == null) return new List<ResortRoomDto>();

        return resort.Rooms
            .Where(r => !r.IsDeleted)
            .Select(MapRoomToDto)
            .ToList();
    }

    public async Task<ResortRoomDto?> CreateRoomAsync(Guid resortId, CreateResortRoomRequest request)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort == null)
            throw new ArgumentException("Resort not found");

        if (request.PricePerNight <= 0)
            throw new ArgumentException("Price per night must be greater than 0");

        var room = new ResortRoom
        {
            RoomType = request.RoomType,
            Description = request.Description,
            MaxOccupancy = request.MaxOccupancy,
            PricePerNight = request.PricePerNight,
            BedType = request.BedType,
            RoomAmenities = request.RoomAmenities != null ? System.Text.Json.JsonSerializer.Serialize(request.RoomAmenities) : null,
            Images = request.Images != null ? System.Text.Json.JsonSerializer.Serialize(request.Images) : null,
            TotalRooms = request.TotalRooms,
            AvailableRooms = request.TotalRooms,
            ResortId = resortId,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.ResortRooms.Add(room);
        
        // Update resort min/max price
        if (resort.MinPrice == null || request.PricePerNight < resort.MinPrice)
            resort.MinPrice = request.PricePerNight;
        if (resort.MaxPrice == null || request.PricePerNight > resort.MaxPrice)
            resort.MaxPrice = request.PricePerNight;

        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapRoomToDto(room);
    }

    public async Task<ResortRoomDto?> UpdateRoomAsync(Guid resortId, Guid roomId, UpdateResortRoomRequest request)
    {
        var room = await _context.ResortRooms
            .FirstOrDefaultAsync(r => r.Id == roomId && r.ResortId == resortId && !r.IsDeleted);

        if (room == null) return null;

        if (request.PricePerNight.HasValue && request.PricePerNight <= 0)
            throw new ArgumentException("Price per night must be greater than 0");

        if (request.RoomType != null) room.RoomType = request.RoomType;
        if (request.Description != null) room.Description = request.Description;
        if (request.MaxOccupancy.HasValue) room.MaxOccupancy = request.MaxOccupancy.Value;
        if (request.PricePerNight.HasValue) room.PricePerNight = request.PricePerNight.Value;
        if (request.BedType != null) room.BedType = request.BedType;
        if (request.RoomAmenities != null) room.RoomAmenities = System.Text.Json.JsonSerializer.Serialize(request.RoomAmenities);
        if (request.Images != null) room.Images = System.Text.Json.JsonSerializer.Serialize(request.Images);
        if (request.TotalRooms.HasValue) room.TotalRooms = request.TotalRooms.Value;
        if (request.AvailableRooms.HasValue) room.AvailableRooms = request.AvailableRooms.Value;

        room.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapRoomToDto(room);
    }

    public async Task<bool> DeleteRoomAsync(Guid resortId, Guid roomId)
    {
        var room = await _context.ResortRooms
            .FirstOrDefaultAsync(r => r.Id == roomId && r.ResortId == resortId && !r.IsDeleted);

        if (room == null) return false;

        // Check for active bookings
        var hasActiveBookings = await _context.Bookings
            .AnyAsync(b => b.ResortRoomId == roomId && 
                          !b.IsDeleted && 
                          b.Status != BookingStatus.Cancelled &&
                          b.EndDate >= DateTime.Now);

        if (hasActiveBookings)
        {
            return false;
        }

        room.IsDeleted = true;
        room.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<RoomAvailabilityDto>> GetRoomAvailabilityAsync(Guid resortId, Guid roomId, DateTime startDate, int days)
    {
        var room = await _context.ResortRooms
            .FirstOrDefaultAsync(r => r.Id == roomId && r.ResortId == resortId && !r.IsDeleted);

        if (room == null) return new List<RoomAvailabilityDto>();

        var endDate = startDate.AddDays(days);
        var availabilities = new List<RoomAvailabilityDto>();
        
        // Get bookings for this period
        var bookings = await _context.Bookings
            .Where(b => b.ResortRoomId == roomId && 
                       !b.IsDeleted &&
                       b.Status != BookingStatus.Cancelled &&
                       b.ServiceDate <= endDate &&
                       b.EndDate >= startDate)
            .ToListAsync();

        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            var bookedRooms = bookings
                .Where(b => b.ServiceDate <= date && date <= (b.EndDate ?? b.ServiceDate))
                .Sum(b => b.Quantity);

            availabilities.Add(new RoomAvailabilityDto
            {
                RoomId = roomId,
                Date = date,
                AvailableRooms = room.AvailableRooms - bookedRooms,
                PriceOverride = null,
                IsHoliday = IsHoliday(date)
            });
        }

        return availabilities;
    }

    public async Task<bool> UpdateRoomAvailabilityAsync(Guid resortId, Guid roomId, UpdateRoomAvailabilityRequest request)
    {
        var room = await _context.ResortRooms
            .FirstOrDefaultAsync(r => r.Id == roomId && r.ResortId == resortId && !r.IsDeleted);

        if (room == null) return false;

        // Update available rooms (simple version)
        if (request.AvailableRooms >= 0 && request.AvailableRooms <= room.TotalRooms)
        {
            room.AvailableRooms = request.AvailableRooms;
            room.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> BulkUpdateRoomAvailabilityAsync(Guid resortId, Guid roomId, BulkUpdateRoomAvailabilityRequest request)
    {
        // This would typically store in a separate table for date-specific pricing
        // For now, we'll just update the base availability
        return await UpdateRoomAvailabilityAsync(resortId, roomId, new UpdateRoomAvailabilityRequest
        {
            AvailableRooms = request.Availabilities.FirstOrDefault()?.AvailableRooms ?? 0
        });
    }

    public async Task<List<RoomPricingDto>> GetRoomPricingAsync(Guid resortId, Guid roomId, DateTime startDate, int days)
    {
        var room = await _context.ResortRooms
            .FirstOrDefaultAsync(r => r.Id == roomId && r.ResortId == resortId && !r.IsDeleted);

        if (room == null) return new List<RoomPricingDto>();

        var availabilities = await GetRoomAvailabilityAsync(resortId, roomId, startDate, days);

        return availabilities.Select(a => new RoomPricingDto
        {
            RoomId = roomId,
            Date = a.Date,
            BasePrice = room.PricePerNight,
            FinalPrice = a.PriceOverride ?? room.PricePerNight,
            DiscountPercentage = a.PriceOverride.HasValue 
                ? (1 - a.PriceOverride.Value / room.PricePerNight) * 100 
                : 0,
            IsAvailable = a.AvailableRooms > 0
        }).ToList();
    }

    public async Task<List<ResortServiceDto>> GetServicesAsync(Guid resortId)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort?.Activities == null) return new List<ResortServiceDto>();

        var activities = System.Text.Json.JsonSerializer.Deserialize<List<ResortServiceDto>>(resort.Activities);
        return activities ?? new List<ResortServiceDto>();
    }

    public async Task<ResortServiceDto> CreateServiceAsync(Guid resortId, CreateResortServiceRequest request)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort == null)
            throw new ArgumentException("Resort not found");

        var services = resort.Activities != null 
            ? System.Text.Json.JsonSerializer.Deserialize<List<ResortServiceDto>>(resort.Activities)
            : new List<ResortServiceDto>();

        var newService = new ResortServiceDto
        {
            ServiceId = Guid.NewGuid(),
            ServiceName = request.ServiceName,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            Category = request.Category,
            IsAvailable = request.IsAvailable,
            Schedule = request.Schedule
        };

        services!.Add(newService);
        resort.Activities = System.Text.Json.JsonSerializer.Serialize(services);
        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return newService;
    }

    public async Task<ResortServiceDto?> UpdateServiceAsync(Guid resortId, Guid serviceId, UpdateResortServiceRequest request)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort?.Activities == null) return null;

        var services = System.Text.Json.JsonSerializer.Deserialize<List<ResortServiceDto>>(resort.Activities);
        var service = services?.FirstOrDefault(s => s.ServiceId == serviceId);

        if (service == null) return null;

        if (request.ServiceName != null) service.ServiceName = request.ServiceName;
        if (request.Description != null) service.Description = request.Description;
        if (request.Price.HasValue) service.Price = request.Price;
        if (request.ImageUrl != null) service.ImageUrl = request.ImageUrl;
        if (request.Category.HasValue) service.Category = request.Category.Value;
        if (request.IsAvailable.HasValue) service.IsAvailable = request.IsAvailable.Value;
        if (request.Schedule != null) service.Schedule = request.Schedule;

        resort.Activities = System.Text.Json.JsonSerializer.Serialize(services);
        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return service;
    }

    public async Task<bool> DeleteServiceAsync(Guid resortId, Guid serviceId)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort?.Activities == null) return false;

        var services = System.Text.Json.JsonSerializer.Deserialize<List<ResortServiceDto>>(resort.Activities);
        var service = services?.FirstOrDefault(s => s.ServiceId == serviceId);

        if (service == null) return false;

        services!.Remove(service);
        resort.Activities = System.Text.Json.JsonSerializer.Serialize(services);
        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ComboPackageDto>> GetPackagesAsync(Guid resortId)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort?.Packages == null) return new List<ComboPackageDto>();

        var packages = System.Text.Json.JsonSerializer.Deserialize<List<ComboPackageDto>>(resort.Packages);
        return packages ?? new List<ComboPackageDto>();
    }

    public async Task<ComboPackageDto> CreatePackageAsync(Guid resortId, CreateComboPackageRequest request)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort == null)
            throw new ArgumentException("Resort not found");

        if (request.TotalPrice <= 0)
            throw new ArgumentException("Total price must be greater than 0");

        var packages = resort.Packages != null 
            ? System.Text.Json.JsonSerializer.Deserialize<List<ComboPackageDto>>(resort.Packages)
            : new List<ComboPackageDto>();

        var newPackage = new ComboPackageDto
        {
            PackageId = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TotalPrice = request.TotalPrice,
            DiscountPrice = request.DiscountPrice,
            RoomType = request.RoomType,
            IncludesMeals = request.IncludesMeals,
            MealPlan = request.MealPlan,
            IncludedServices = request.IncludedServices,
            IsAvailable = request.IsAvailable,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };

        packages!.Add(newPackage);
        resort.Packages = System.Text.Json.JsonSerializer.Serialize(packages);
        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return newPackage;
    }

    public async Task<ComboPackageDto?> UpdatePackageAsync(Guid resortId, Guid packageId, UpdateComboPackageRequest request)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort?.Packages == null) return null;

        var packages = System.Text.Json.JsonSerializer.Deserialize<List<ComboPackageDto>>(resort.Packages);
        var package = packages?.FirstOrDefault(p => p.PackageId == packageId);

        if (package == null) return null;

        if (request.TotalPrice.HasValue && request.TotalPrice <= 0)
            throw new ArgumentException("Total price must be greater than 0");

        if (request.Name != null) package.Name = request.Name;
        if (request.Description != null) package.Description = request.Description;
        if (request.TotalPrice.HasValue) package.TotalPrice = request.TotalPrice.Value;
        if (request.DiscountPrice.HasValue) package.DiscountPrice = request.DiscountPrice;
        if (request.RoomType != null) package.RoomType = request.RoomType;
        if (request.IncludesMeals.HasValue) package.IncludesMeals = request.IncludesMeals.Value;
        if (request.MealPlan.HasValue) package.MealPlan = request.MealPlan.Value;
        if (request.IncludedServices != null) package.IncludedServices = request.IncludedServices;
        if (request.IsAvailable.HasValue) package.IsAvailable = request.IsAvailable.Value;
        if (request.ValidFrom.HasValue) package.ValidFrom = request.ValidFrom;
        if (request.ValidTo.HasValue) package.ValidTo = request.ValidTo;

        resort.Packages = System.Text.Json.JsonSerializer.Serialize(packages);
        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return package;
    }

    public async Task<bool> DeletePackageAsync(Guid resortId, Guid packageId)
    {
        var resort = await _context.Resorts
            .FirstOrDefaultAsync(r => r.Id == resortId && !r.IsDeleted);

        if (resort?.Packages == null) return false;

        var packages = System.Text.Json.JsonSerializer.Deserialize<List<ComboPackageDto>>(resort.Packages);
        var package = packages?.FirstOrDefault(p => p.PackageId == packageId);

        if (package == null) return false;

        packages!.Remove(package);
        resort.Packages = System.Text.Json.JsonSerializer.Serialize(packages);
        resort.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ResortDto>> SearchAsync(string? searchTerm, ResortType? resortType, List<ResortAmenity>? amenities, string? city)
    {
        var query = _context.Resorts.Where(r => !r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(term) || 
                                     (r.Description != null && r.Description.ToLower().Contains(term)));
        }

        if (resortType.HasValue)
        {
            query = query.Where(r => r.LocationType == resortType.Value.ToString());
        }

        if (amenities != null && amenities.Any())
        {
            var amenityStrings = amenities.Select(a => a.ToString()).ToList();
            // This would require more complex JSON querying
            // For now, we'll do a simple string contains
            foreach (var amenity in amenityStrings)
            {
                query = query.Where(r => r.Amenities != null && r.Amenities.Contains(amenity));
            }
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(r => r.City.ToLower() == city.ToLower());
        }

        var resorts = await query
            .OrderByDescending(r => r.IsFeatured)
            .ThenByDescending(r => r.Rating)
            .ToListAsync();

        return resorts.Select(MapToDto).ToList();
    }

    private ResortDto MapToDto(Resort resort)
    {
        List<string>? images = null;
        if (!string.IsNullOrEmpty(resort.Images))
        {
            images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(resort.Images);
        }

        List<string>? amenities = null;
        if (!string.IsNullOrEmpty(resort.Amenities))
        {
            amenities = System.Text.Json.JsonSerializer.Deserialize<List<string>>(resort.Amenities);
        }

        List<string>? activities = null;
        if (!string.IsNullOrEmpty(resort.Activities))
        {
            activities = System.Text.Json.JsonSerializer.Deserialize<List<string>>(resort.Activities);
        }

        List<ComboPackageDto>? packages = null;
        if (!string.IsNullOrEmpty(resort.Packages))
        {
            packages = System.Text.Json.JsonSerializer.Deserialize<List<ComboPackageDto>>(resort.Packages);
        }

        return new ResortDto
        {
            ResortId = resort.Id,
            Name = resort.Name,
            Description = resort.Description,
            Address = resort.Address,
            City = resort.City,
            LocationType = resort.LocationType,
            StarRating = resort.StarRating,
            Images = images,
            MinPrice = resort.MinPrice,
            MaxPrice = resort.MaxPrice,
            Amenities = amenities,
            Activities = activities,
            Services = null,
            Packages = packages,
            Rooms = resort.Rooms?.Where(r => !r.IsDeleted).Select(MapRoomToDto).ToList(),
            Rating = resort.Rating,
            ReviewCount = resort.ReviewCount,
            IsFeatured = resort.IsFeatured,
            CreatedAt = resort.CreatedAt
        };
    }

    private ResortRoomDto MapRoomToDto(ResortRoom room)
    {
        List<string>? roomAmenities = null;
        if (!string.IsNullOrEmpty(room.RoomAmenities))
        {
            roomAmenities = System.Text.Json.JsonSerializer.Deserialize<List<string>>(room.RoomAmenities);
        }

        List<string>? images = null;
        if (!string.IsNullOrEmpty(room.Images))
        {
            images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(room.Images);
        }

        return new ResortRoomDto
        {
            RoomId = room.Id,
            RoomType = room.RoomType,
            Description = room.Description,
            MaxOccupancy = room.MaxOccupancy,
            PricePerNight = room.PricePerNight,
            BedType = room.BedType,
            RoomAmenities = roomAmenities,
            Images = images,
            TotalRooms = room.TotalRooms,
            AvailableRooms = room.AvailableRooms,
            IsAvailable = room.AvailableRooms > 0
        };
    }

    private bool IsHoliday(DateTime date)
    {
        // Simple holiday check - could be expanded
        var vietnamHolidays = new[]
        {
            new DateTime(date.Year, 1, 1),   // New Year
            new DateTime(date.Year, 4, 30),  // Reunification Day
            new DateTime(date.Year, 5, 1),   // Labor Day
            new DateTime(date.Year, 9, 2)    // Independence Day
        };
        
        return vietnamHolidays.Any(h => h.Month == date.Month && h.Day == date.Day);
    }
}
