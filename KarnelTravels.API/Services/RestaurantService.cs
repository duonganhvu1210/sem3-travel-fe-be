using KarnelTravels.API.Data;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Services;

public interface IRestaurantService
{
    // Restaurant CRUD
    Task<List<RestaurantDto>> GetAllAsync();
    Task<RestaurantDto?> GetByIdAsync(Guid id);
    Task<RestaurantDto> CreateAsync(CreateRestaurantRequest request);
    Task<RestaurantDto?> UpdateAsync(Guid id, UpdateRestaurantRequest request);
    Task<bool> DeleteAsync(Guid id);
    
    // Hours Management
    Task<RestaurantDto?> UpdateHoursAsync(Guid id, UpdateRestaurantHoursRequest request);
    
    // Menu Management
    Task<List<MenuItemDto>> GetMenuAsync(Guid restaurantId);
    Task<MenuItemDto?> CreateMenuItemAsync(Guid restaurantId, CreateMenuItemRequest request);
    Task<MenuItemDto?> UpdateMenuItemAsync(Guid restaurantId, Guid menuItemId, UpdateMenuItemRequest request);
    Task<bool> DeleteMenuItemAsync(Guid restaurantId, Guid menuItemId);
    
    // Reservations
    Task<List<TableReservationDto>> GetReservationsAsync(Guid restaurantId);
    Task<TableReservationDto?> GetReservationByIdAsync(Guid restaurantId, Guid reservationId);
    Task<TableReservationDto> CreateReservationAsync(CreateTableReservationRequest request);
    Task<TableReservationDto?> UpdateReservationStatusAsync(Guid restaurantId, Guid reservationId, UpdateReservationStatusRequest request);
    Task<bool> DeleteReservationAsync(Guid restaurantId, Guid reservationId);
    
    // Search & Filter
    Task<List<RestaurantDto>> SearchAsync(string? searchTerm, CuisineType? cuisineType, PriceRange? priceRange, string? city);
}

public class RestaurantService : IRestaurantService
{
    private readonly KarnelTravelsDbContext _context;

    public RestaurantService(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    public async Task<List<RestaurantDto>> GetAllAsync()
    {
        var restaurants = await _context.Restaurants
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.IsFeatured)
            .ThenByDescending(r => r.Rating)
            .ToListAsync();

        return restaurants.Select(MapToDto).ToList();
    }

    public async Task<RestaurantDto?> GetByIdAsync(Guid id)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        return restaurant == null ? null : MapToDto(restaurant);
    }

    public async Task<RestaurantDto> CreateAsync(CreateRestaurantRequest request)
    {
        // Validate hours
        if (!string.IsNullOrEmpty(request.OpeningTime) && !string.IsNullOrEmpty(request.ClosingTime))
        {
            if (!TimeSpan.TryParse(request.OpeningTime, out var openTime) || 
                !TimeSpan.TryParse(request.ClosingTime, out var closeTime))
            {
                throw new ArgumentException("Invalid time format. Use HH:mm format.");
            }
            
            if (openTime >= closeTime)
            {
                throw new ArgumentException("Opening time must be before closing time.");
            }
        }

        var restaurant = new Restaurant
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            CuisineType = request.CuisineType.ToString(),
            PriceLevel = request.PriceLevel.ToString(),
            Style = request.Style.ToString(),
            OpeningTime = request.OpeningTime,
            ClosingTime = request.ClosingTime,
            ContactPhone = request.ContactPhone,
            Images = request.Images != null ? System.Text.Json.JsonSerializer.Serialize(request.Images) : null,
            Menu = request.Menu != null ? System.Text.Json.JsonSerializer.Serialize(request.Menu) : null,
            HasReservation = request.HasReservation,
            IsFeatured = request.IsFeatured,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        return MapToDto(restaurant);
    }

    public async Task<RestaurantDto?> UpdateAsync(Guid id, UpdateRestaurantRequest request)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (restaurant == null) return null;

        // Validate hours if being updated
        var openingTime = request.OpeningTime ?? restaurant.OpeningTime;
        var closingTime = request.ClosingTime ?? restaurant.ClosingTime;
        
        if (!string.IsNullOrEmpty(openingTime) && !string.IsNullOrEmpty(closingTime))
        {
            if (!TimeSpan.TryParse(openingTime, out var openTime) || 
                !TimeSpan.TryParse(closingTime, out var closeTime))
            {
                throw new ArgumentException("Invalid time format. Use HH:mm format.");
            }
            
            if (openTime >= closeTime)
            {
                throw new ArgumentException("Opening time must be before closing time.");
            }
        }

        if (request.Name != null) restaurant.Name = request.Name;
        if (request.Description != null) restaurant.Description = request.Description;
        if (request.Address != null) restaurant.Address = request.Address;
        if (request.City != null) restaurant.City = request.City;
        if (request.CuisineType.HasValue) restaurant.CuisineType = request.CuisineType.Value.ToString();
        if (request.PriceLevel.HasValue) restaurant.PriceLevel = request.PriceLevel.Value.ToString();
        if (request.Style.HasValue) restaurant.Style = request.Style.Value.ToString();
        if (request.OpeningTime != null) restaurant.OpeningTime = request.OpeningTime;
        if (request.ClosingTime != null) restaurant.ClosingTime = request.ClosingTime;
        if (request.ContactPhone != null) restaurant.ContactPhone = request.ContactPhone;
        if (request.Images != null) restaurant.Images = System.Text.Json.JsonSerializer.Serialize(request.Images);
        if (request.HasReservation.HasValue) restaurant.HasReservation = request.HasReservation.Value;
        if (request.IsFeatured.HasValue) restaurant.IsFeatured = request.IsFeatured.Value;

        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(restaurant);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (restaurant == null) return false;

        // Check for active bookings
        var hasActiveBookings = await _context.Bookings
            .AnyAsync(b => b.RestaurantId == id && 
                          !b.IsDeleted && 
                          b.Status != BookingStatus.Cancelled &&
                          b.ServiceDate >= DateTime.Now);

        if (hasActiveBookings)
        {
            return false;
        }

        restaurant.IsDeleted = true;
        restaurant.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<RestaurantDto?> UpdateHoursAsync(Guid id, UpdateRestaurantHoursRequest request)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (restaurant == null) return null;

        var openingTime = request.OpeningTime ?? restaurant.OpeningTime;
        var closingTime = request.ClosingTime ?? restaurant.ClosingTime;

        if (!string.IsNullOrEmpty(openingTime) && !string.IsNullOrEmpty(closingTime))
        {
            if (!TimeSpan.TryParse(openingTime, out var openTime) || 
                !TimeSpan.TryParse(closingTime, out var closeTime))
            {
                throw new ArgumentException("Invalid time format. Use HH:mm format.");
            }
            
            if (openTime >= closeTime)
            {
                throw new ArgumentException("Opening time must be before closing time.");
            }
        }

        if (request.OpeningTime != null) restaurant.OpeningTime = request.OpeningTime;
        if (request.ClosingTime != null) restaurant.ClosingTime = request.ClosingTime;

        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(restaurant);
    }

    public async Task<List<MenuItemDto>> GetMenuAsync(Guid restaurantId)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId && !r.IsDeleted);

        if (restaurant?.Menu == null) return new List<MenuItemDto>();

        var menuItems = System.Text.Json.JsonSerializer.Deserialize<List<MenuItemDto>>(restaurant.Menu);
        return menuItems ?? new List<MenuItemDto>();
    }

    public async Task<MenuItemDto> CreateMenuItemAsync(Guid restaurantId, CreateMenuItemRequest request)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId && !r.IsDeleted);

        if (restaurant == null)
            throw new ArgumentException("Restaurant not found");

        var menu = restaurant.Menu != null 
            ? System.Text.Json.JsonSerializer.Deserialize<List<MenuItemDto>>(restaurant.Menu)
            : new List<MenuItemDto>();

        var newItem = new MenuItemDto
        {
            MenuItemId = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            Category = request.Category,
            IsAvailable = request.IsAvailable,
            IsVegetarian = request.IsVegetarian,
            IsSpicy = request.IsSpicy
        };

        menu!.Add(newItem);
        restaurant.Menu = System.Text.Json.JsonSerializer.Serialize(menu);
        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return newItem;
    }

    public async Task<MenuItemDto?> UpdateMenuItemAsync(Guid restaurantId, Guid menuItemId, UpdateMenuItemRequest request)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId && !r.IsDeleted);

        if (restaurant?.Menu == null) return null;

        var menu = System.Text.Json.JsonSerializer.Deserialize<List<MenuItemDto>>(restaurant.Menu);
        var item = menu?.FirstOrDefault(m => m.MenuItemId == menuItemId);

        if (item == null) return null;

        if (request.Name != null) item.Name = request.Name;
        if (request.Description != null) item.Description = request.Description;
        if (request.Price.HasValue) item.Price = request.Price.Value;
        if (request.ImageUrl != null) item.ImageUrl = request.ImageUrl;
        if (request.Category != null) item.Category = request.Category;
        if (request.IsAvailable.HasValue) item.IsAvailable = request.IsAvailable.Value;
        if (request.IsVegetarian.HasValue) item.IsVegetarian = request.IsVegetarian.Value;
        if (request.IsSpicy.HasValue) item.IsSpicy = request.IsSpicy.Value;

        restaurant.Menu = System.Text.Json.JsonSerializer.Serialize(menu);
        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<bool> DeleteMenuItemAsync(Guid restaurantId, Guid menuItemId)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId && !r.IsDeleted);

        if (restaurant?.Menu == null) return false;

        var menu = System.Text.Json.JsonSerializer.Deserialize<List<MenuItemDto>>(restaurant.Menu);
        var item = menu?.FirstOrDefault(m => m.MenuItemId == menuItemId);

        if (item == null) return false;

        menu!.Remove(item);
        restaurant.Menu = System.Text.Json.JsonSerializer.Serialize(menu);
        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<TableReservationDto>> GetReservationsAsync(Guid restaurantId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.RestaurantId == restaurantId && !b.IsDeleted && b.Type == BookingType.Restaurant)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookings.Select(MapBookingToReservationDto).ToList();
    }

    public async Task<TableReservationDto?> GetReservationByIdAsync(Guid restaurantId, Guid reservationId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == reservationId && 
                                      b.RestaurantId == restaurantId && 
                                      !b.IsDeleted &&
                                      b.Type == BookingType.Restaurant);

        return booking == null ? null : MapBookingToReservationDto(booking);
    }

    public async Task<TableReservationDto> CreateReservationAsync(CreateTableReservationRequest request)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId && !r.IsDeleted);

        if (restaurant == null)
            throw new ArgumentException("Restaurant not found");

        var booking = new Booking
        {
            BookingCode = GenerateBookingCode(),
            Type = BookingType.Restaurant,
            Status = BookingStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            ServiceDate = request.ReservationDate,
            ContactName = request.CustomerName,
            ContactEmail = request.CustomerEmail,
            ContactPhone = request.CustomerPhone,
            Quantity = request.PartySize,
            Notes = request.Notes,
            RestaurantId = request.RestaurantId,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddHours(24)
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return MapBookingToReservationDto(booking);
    }

    public async Task<TableReservationDto?> UpdateReservationStatusAsync(Guid restaurantId, Guid reservationId, UpdateReservationStatusRequest request)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == reservationId && 
                                      b.RestaurantId == restaurantId && 
                                      !b.IsDeleted &&
                                      b.Type == BookingType.Restaurant);

        if (booking == null) return null;

        if (Enum.TryParse<BookingStatus>(request.Status, true, out var status))
        {
            booking.Status = status;
            
            if (status == BookingStatus.Cancelled)
            {
                booking.CancelledAt = DateTime.UtcNow;
            }
            else if (status == BookingStatus.Completed)
            {
                booking.PaidAt = DateTime.UtcNow;
                booking.PaymentStatus = PaymentStatus.Paid;
            }
        }

        booking.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapBookingToReservationDto(booking);
    }

    public async Task<bool> DeleteReservationAsync(Guid restaurantId, Guid reservationId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == reservationId && 
                                      b.RestaurantId == restaurantId && 
                                      !b.IsDeleted &&
                                      b.Type == BookingType.Restaurant);

        if (booking == null) return false;

        booking.IsDeleted = true;
        booking.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<RestaurantDto>> SearchAsync(string? searchTerm, CuisineType? cuisineType, PriceRange? priceRange, string? city)
    {
        var query = _context.Restaurants.Where(r => !r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(term) || 
                                     (r.Description != null && r.Description.ToLower().Contains(term)));
        }

        if (cuisineType.HasValue)
        {
            query = query.Where(r => r.CuisineType == cuisineType.Value.ToString());
        }

        if (priceRange.HasValue)
        {
            query = query.Where(r => r.PriceLevel == priceRange.Value.ToString());
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(r => r.City.ToLower() == city.ToLower());
        }

        var restaurants = await query
            .OrderByDescending(r => r.IsFeatured)
            .ThenByDescending(r => r.Rating)
            .ToListAsync();

        return restaurants.Select(MapToDto).ToList();
    }

    private RestaurantDto MapToDto(Restaurant restaurant)
    {
        List<string>? images = null;
        if (!string.IsNullOrEmpty(restaurant.Images))
        {
            images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(restaurant.Images);
        }

        List<MenuItemDto>? menu = null;
        if (!string.IsNullOrEmpty(restaurant.Menu))
        {
            menu = System.Text.Json.JsonSerializer.Deserialize<List<MenuItemDto>>(restaurant.Menu);
        }

        return new RestaurantDto
        {
            RestaurantId = restaurant.Id,
            Name = restaurant.Name,
            Description = restaurant.Description,
            Address = restaurant.Address,
            City = restaurant.City,
            CuisineType = restaurant.CuisineType,
            PriceLevel = restaurant.PriceLevel,
            Style = restaurant.Style,
            OpeningTime = restaurant.OpeningTime,
            ClosingTime = restaurant.ClosingTime,
            ContactPhone = restaurant.ContactPhone,
            Images = images,
            Menu = menu,
            HasReservation = restaurant.HasReservation,
            Rating = restaurant.Rating,
            ReviewCount = restaurant.ReviewCount,
            IsFeatured = restaurant.IsFeatured,
            CreatedAt = restaurant.CreatedAt
        };
    }

    private TableReservationDto MapBookingToReservationDto(Booking booking)
    {
        return new TableReservationDto
        {
            ReservationId = booking.Id,
            CustomerName = booking.ContactName,
            CustomerEmail = booking.ContactEmail,
            CustomerPhone = booking.ContactPhone,
            PartySize = booking.Quantity,
            ReservationDate = booking.ServiceDate ?? DateTime.MinValue,
            ReservationTime = booking.ServiceDate?.ToString("HH:mm") ?? "",
            Notes = booking.Notes,
            Status = booking.Status.ToString(),
            CreatedAt = booking.CreatedAt
        };
    }

    private string GenerateBookingCode()
    {
        return $"REST-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
