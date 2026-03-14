using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.EntityFrameworkCore;
using Route = KarnelTravels.API.Entities.Route;
using Vehicle = KarnelTravels.API.Entities.Vehicle;
using Schedule = KarnelTravels.API.Entities.Schedule;
using VehicleType = KarnelTravels.API.Entities.VehicleType;
using TransportProvider = KarnelTravels.API.Entities.TransportProvider;

namespace KarnelTravels.API.Services;

// ==================== VehicleType Service ====================

public interface IVehicleTypeService
{
    Task<List<VehicleTypeDto>> GetAllAsync();
    Task<VehicleTypeDto?> GetByIdAsync(Guid id);
    Task<VehicleTypeDto> CreateAsync(CreateVehicleTypeRequest request);
    Task<VehicleTypeDto?> UpdateAsync(Guid id, CreateVehicleTypeRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class VehicleTypeService : IVehicleTypeService
{
    private readonly KarnelTravelsDbContext _context;

    public VehicleTypeService(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleTypeDto>> GetAllAsync()
    {
        return await _context.VehicleTypes
            .Where(vt => !vt.IsDeleted)
            .OrderBy(vt => vt.Name)
            .Select(vt => new VehicleTypeDto
            {
                VehicleTypeId = vt.VehicleTypeId,
                Name = vt.Name,
                Description = vt.Description,
                Icon = vt.Icon,
                CreatedAt = vt.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<VehicleTypeDto?> GetByIdAsync(Guid id)
    {
        var vehicleType = await _context.VehicleTypes
            .FirstOrDefaultAsync(vt => vt.VehicleTypeId == id && !vt.IsDeleted);

        if (vehicleType == null) return null;

        return new VehicleTypeDto
        {
            VehicleTypeId = vehicleType.VehicleTypeId,
            Name = vehicleType.Name,
            Description = vehicleType.Description,
            Icon = vehicleType.Icon,
            CreatedAt = vehicleType.CreatedAt
        };
    }

    public async Task<VehicleTypeDto> CreateAsync(CreateVehicleTypeRequest request)
    {
        var vehicleType = new VehicleType
        {
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon
        };

        _context.VehicleTypes.Add(vehicleType);
        await _context.SaveChangesAsync();

        return new VehicleTypeDto
        {
            VehicleTypeId = vehicleType.VehicleTypeId,
            Name = vehicleType.Name,
            Description = vehicleType.Description,
            Icon = vehicleType.Icon,
            CreatedAt = vehicleType.CreatedAt
        };
    }

    public async Task<VehicleTypeDto?> UpdateAsync(Guid id, CreateVehicleTypeRequest request)
    {
        var vehicleType = await _context.VehicleTypes
            .FirstOrDefaultAsync(vt => vt.VehicleTypeId == id && !vt.IsDeleted);

        if (vehicleType == null) return null;

        vehicleType.Name = request.Name;
        vehicleType.Description = request.Description;
        vehicleType.Icon = request.Icon;

        await _context.SaveChangesAsync();

        return new VehicleTypeDto
        {
            VehicleTypeId = vehicleType.VehicleTypeId,
            Name = vehicleType.Name,
            Description = vehicleType.Description,
            Icon = vehicleType.Icon,
            CreatedAt = vehicleType.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var vehicleType = await _context.VehicleTypes
            .FirstOrDefaultAsync(vt => vt.VehicleTypeId == id && !vt.IsDeleted);

        if (vehicleType == null) return false;

        vehicleType.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}

// ==================== TransportProvider Service ====================

public interface ITransportProviderService
{
    Task<List<TransportProviderDto>> GetAllAsync();
    Task<TransportProviderDto?> GetByIdAsync(Guid id);
    Task<TransportProviderDto> CreateAsync(CreateTransportProviderRequest request);
    Task<TransportProviderDto?> UpdateAsync(Guid id, CreateTransportProviderRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class TransportProviderService : ITransportProviderService
{
    private readonly KarnelTravelsDbContext _context;

    public TransportProviderService(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransportProviderDto>> GetAllAsync()
    {
        return await _context.TransportProviders
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Name)
            .Select(p => new TransportProviderDto
            {
                ProviderId = p.ProviderId,
                Name = p.Name,
                Description = p.Description,
                ContactPhone = p.ContactPhone,
                ContactEmail = p.ContactEmail,
                ContactAddress = p.ContactAddress,
                Website = p.Website,
                LogoUrl = p.LogoUrl,
                VehicleCount = p.Vehicles.Count(v => !v.IsDeleted),
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<TransportProviderDto?> GetByIdAsync(Guid id)
    {
        var provider = await _context.TransportProviders
            .FirstOrDefaultAsync(p => p.ProviderId == id && !p.IsDeleted);

        if (provider == null) return null;

        return new TransportProviderDto
        {
            ProviderId = provider.ProviderId,
            Name = provider.Name,
            Description = provider.Description,
            ContactPhone = provider.ContactPhone,
            ContactEmail = provider.ContactEmail,
            ContactAddress = provider.ContactAddress,
            Website = provider.Website,
            LogoUrl = provider.LogoUrl,
            VehicleCount = provider.Vehicles.Count(v => !v.IsDeleted),
            CreatedAt = provider.CreatedAt
        };
    }

    public async Task<TransportProviderDto> CreateAsync(CreateTransportProviderRequest request)
    {
        var provider = new TransportProvider
        {
            Name = request.Name,
            Description = request.Description,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail,
            ContactAddress = request.ContactAddress,
            Website = request.Website,
            LogoUrl = request.LogoUrl
        };

        _context.TransportProviders.Add(provider);
        await _context.SaveChangesAsync();

        return new TransportProviderDto
        {
            ProviderId = provider.ProviderId,
            Name = provider.Name,
            Description = provider.Description,
            ContactPhone = provider.ContactPhone,
            ContactEmail = provider.ContactEmail,
            ContactAddress = provider.ContactAddress,
            Website = provider.Website,
            LogoUrl = provider.LogoUrl,
            VehicleCount = 0,
            CreatedAt = provider.CreatedAt
        };
    }

    public async Task<TransportProviderDto?> UpdateAsync(Guid id, CreateTransportProviderRequest request)
    {
        var provider = await _context.TransportProviders
            .FirstOrDefaultAsync(p => p.ProviderId == id && !p.IsDeleted);

        if (provider == null) return null;

        provider.Name = request.Name;
        provider.Description = request.Description;
        provider.ContactPhone = request.ContactPhone;
        provider.ContactEmail = request.ContactEmail;
        provider.ContactAddress = request.ContactAddress;
        provider.Website = request.Website;
        provider.LogoUrl = request.LogoUrl;

        await _context.SaveChangesAsync();

        return new TransportProviderDto
        {
            ProviderId = provider.ProviderId,
            Name = provider.Name,
            Description = provider.Description,
            ContactPhone = provider.ContactPhone,
            ContactEmail = provider.ContactEmail,
            ContactAddress = provider.ContactAddress,
            Website = provider.Website,
            LogoUrl = provider.LogoUrl,
            VehicleCount = provider.Vehicles.Count(v => !v.IsDeleted),
            CreatedAt = provider.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var provider = await _context.TransportProviders
            .FirstOrDefaultAsync(p => p.ProviderId == id && !p.IsDeleted);

        if (provider == null) return false;

        provider.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}

// ==================== Route Service ====================

public interface IRouteService
{
    Task<List<RouteDto>> GetAllAsync();
    Task<RouteDto?> GetByIdAsync(Guid id);
    Task<RouteDto> CreateAsync(CreateRouteRequest request);
    Task<RouteDto?> UpdateAsync(Guid id, CreateRouteRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<List<string>> GetDistinctDepartureLocationsAsync();
    Task<List<string>> GetDistinctArrivalLocationsAsync();
}

public class RouteService : IRouteService
{
    private readonly KarnelTravelsDbContext _context;

    public RouteService(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    public async Task<List<RouteDto>> GetAllAsync()
    {
        return await _context.Routes
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.DepartureLocation)
            .ThenBy(r => r.ArrivalLocation)
            .Select(r => new RouteDto
            {
                RouteId = r.RouteId,
                DepartureLocation = r.DepartureLocation,
                ArrivalLocation = r.ArrivalLocation,
                RouteName = r.RouteName,
                Distance = r.Distance,
                Description = r.Description,
                EstimatedDuration = r.EstimatedDuration,
                ScheduleCount = r.Schedules.Count(s => !s.IsDeleted),
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<RouteDto?> GetByIdAsync(Guid id)
    {
        var route = await _context.Routes
            .FirstOrDefaultAsync(r => r.RouteId == id && !r.IsDeleted);

        if (route == null) return null;

        return new RouteDto
        {
            RouteId = route.RouteId,
            DepartureLocation = route.DepartureLocation,
            ArrivalLocation = route.ArrivalLocation,
            RouteName = route.RouteName,
            Distance = route.Distance,
            Description = route.Description,
            EstimatedDuration = route.EstimatedDuration,
            ScheduleCount = route.Schedules.Count(s => !s.IsDeleted),
            CreatedAt = route.CreatedAt
        };
    }

    public async Task<RouteDto> CreateAsync(CreateRouteRequest request)
    {
        var route = new Route
        {
            DepartureLocation = request.DepartureLocation,
            ArrivalLocation = request.ArrivalLocation,
            RouteName = request.RouteName ?? $"{request.DepartureLocation} - {request.ArrivalLocation}",
            Distance = request.Distance,
            Description = request.Description,
            EstimatedDuration = request.EstimatedDuration
        };

        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        return new RouteDto
        {
            RouteId = route.RouteId,
            DepartureLocation = route.DepartureLocation,
            ArrivalLocation = route.ArrivalLocation,
            RouteName = route.RouteName,
            Distance = route.Distance,
            Description = route.Description,
            EstimatedDuration = route.EstimatedDuration,
            ScheduleCount = 0,
            CreatedAt = route.CreatedAt
        };
    }

    public async Task<RouteDto?> UpdateAsync(Guid id, CreateRouteRequest request)
    {
        var route = await _context.Routes
            .FirstOrDefaultAsync(r => r.RouteId == id && !r.IsDeleted);

        if (route == null) return null;

        route.DepartureLocation = request.DepartureLocation;
        route.ArrivalLocation = request.ArrivalLocation;
        route.RouteName = request.RouteName ?? $"{request.DepartureLocation} - {request.ArrivalLocation}";
        route.Distance = request.Distance;
        route.Description = request.Description;
        route.EstimatedDuration = request.EstimatedDuration;

        await _context.SaveChangesAsync();

        return new RouteDto
        {
            RouteId = route.RouteId,
            DepartureLocation = route.DepartureLocation,
            ArrivalLocation = route.ArrivalLocation,
            RouteName = route.RouteName,
            Distance = route.Distance,
            Description = route.Description,
            EstimatedDuration = route.EstimatedDuration,
            ScheduleCount = route.Schedules.Count(s => !s.IsDeleted),
            CreatedAt = route.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var route = await _context.Routes
            .FirstOrDefaultAsync(r => r.RouteId == id && !r.IsDeleted);

        if (route == null) return false;

        route.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<string>> GetDistinctDepartureLocationsAsync()
    {
        return await _context.Routes
            .Where(r => !r.IsDeleted)
            .Select(r => r.DepartureLocation)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();
    }

    public async Task<List<string>> GetDistinctArrivalLocationsAsync()
    {
        return await _context.Routes
            .Where(r => !r.IsDeleted)
            .Select(r => r.ArrivalLocation)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();
    }
}

// ==================== Vehicle Service ====================

public interface IVehicleService
{
    Task<List<VehicleDto>> GetAllAsync();
    Task<VehicleDto?> GetByIdAsync(Guid id);
    Task<VehicleDto> CreateAsync(CreateVehicleRequest request);
    Task<VehicleDto?> UpdateAsync(Guid id, CreateVehicleRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, VehicleStatus status);
    Task<List<VehicleDto>> GetByProviderAsync(Guid providerId);
    Task<List<VehicleDto>> GetByTypeAsync(Guid vehicleTypeId);
}

public class VehicleService : IVehicleService
{
    private readonly KarnelTravelsDbContext _context;

    public VehicleService(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleDto>> GetAllAsync()
    {
        return await _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.Provider)
            .Where(v => !v.IsDeleted)
            .OrderBy(v => v.Name)
            .Select(v => new VehicleDto
            {
                VehicleId = v.VehicleId,
                Name = v.Name,
                LicensePlate = v.LicensePlate,
                Capacity = v.Capacity,
                VehicleTypeId = v.VehicleTypeId,
                VehicleTypeName = v.VehicleType != null ? v.VehicleType.Name : "",
                ProviderId = v.ProviderId,
                ProviderName = v.Provider != null ? v.Provider.Name : "",
                Description = v.Description,
                ImageUrl = v.ImageUrl,
                Amenities = string.IsNullOrEmpty(v.Amenities) ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v.Amenities, new System.Text.Json.JsonSerializerOptions()),
                Status = v.Status.ToString(),
                ScheduleCount = v.Schedules.Count(s => !s.IsDeleted),
                CreatedAt = v.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<VehicleDto?> GetByIdAsync(Guid id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.Provider)
            .FirstOrDefaultAsync(v => v.VehicleId == id && !v.IsDeleted);

        if (vehicle == null) return null;

        return new VehicleDto
        {
            VehicleId = vehicle.VehicleId,
            Name = vehicle.Name,
            LicensePlate = vehicle.LicensePlate,
            Capacity = vehicle.Capacity,
            VehicleTypeId = vehicle.VehicleTypeId,
            VehicleTypeName = vehicle.VehicleType?.Name ?? "",
            ProviderId = vehicle.ProviderId,
            ProviderName = vehicle.Provider?.Name ?? "",
            Description = vehicle.Description,
            ImageUrl = vehicle.ImageUrl,
            Amenities = string.IsNullOrEmpty(vehicle.Amenities) ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(vehicle.Amenities, new System.Text.Json.JsonSerializerOptions()),
            Status = vehicle.Status.ToString(),
            ScheduleCount = vehicle.Schedules.Count(s => !s.IsDeleted),
            CreatedAt = vehicle.CreatedAt
        };
    }

    public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request)
    {
        var vehicle = new Vehicle
        {
            Name = request.Name,
            LicensePlate = request.LicensePlate,
            Capacity = request.Capacity,
            VehicleTypeId = request.VehicleTypeId,
            ProviderId = request.ProviderId,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Amenities = request.Amenities != null ? System.Text.Json.JsonSerializer.Serialize(request.Amenities) : null,
            Status = Enum.TryParse<VehicleStatus>(request.Status, true, out var status) ? status : VehicleStatus.Available
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        var vehicleType = await _context.VehicleTypes.FindAsync(vehicle.VehicleTypeId);
        var provider = await _context.TransportProviders.FindAsync(vehicle.ProviderId);

        return new VehicleDto
        {
            VehicleId = vehicle.VehicleId,
            Name = vehicle.Name,
            LicensePlate = vehicle.LicensePlate,
            Capacity = vehicle.Capacity,
            VehicleTypeId = vehicle.VehicleTypeId,
            VehicleTypeName = vehicleType?.Name ?? "",
            ProviderId = vehicle.ProviderId,
            ProviderName = provider?.Name ?? "",
            Description = vehicle.Description,
            ImageUrl = vehicle.ImageUrl,
            Amenities = request.Amenities,
            Status = vehicle.Status.ToString(),
            ScheduleCount = 0,
            CreatedAt = vehicle.CreatedAt
        };
    }

    public async Task<VehicleDto?> UpdateAsync(Guid id, CreateVehicleRequest request)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == id && !v.IsDeleted);

        if (vehicle == null) return null;

        vehicle.Name = request.Name;
        vehicle.LicensePlate = request.LicensePlate;
        vehicle.Capacity = request.Capacity;
        vehicle.VehicleTypeId = request.VehicleTypeId;
        vehicle.ProviderId = request.ProviderId;
        vehicle.Description = request.Description;
        vehicle.ImageUrl = request.ImageUrl;
        vehicle.Amenities = request.Amenities != null ? System.Text.Json.JsonSerializer.Serialize(request.Amenities) : null;
        vehicle.Status = Enum.TryParse<VehicleStatus>(request.Status, true, out var status) ? status : VehicleStatus.Available;

        await _context.SaveChangesAsync();

        var vehicleType = await _context.VehicleTypes.FindAsync(vehicle.VehicleTypeId);
        var provider = await _context.TransportProviders.FindAsync(vehicle.ProviderId);

        return new VehicleDto
        {
            VehicleId = vehicle.VehicleId,
            Name = vehicle.Name,
            LicensePlate = vehicle.LicensePlate,
            Capacity = vehicle.Capacity,
            VehicleTypeId = vehicle.VehicleTypeId,
            VehicleTypeName = vehicleType?.Name ?? "",
            ProviderId = vehicle.ProviderId,
            ProviderName = provider?.Name ?? "",
            Description = vehicle.Description,
            ImageUrl = vehicle.ImageUrl,
            Amenities = request.Amenities,
            Status = vehicle.Status.ToString(),
            ScheduleCount = vehicle.Schedules.Count(s => !s.IsDeleted),
            CreatedAt = vehicle.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == id && !v.IsDeleted);

        if (vehicle == null) return false;

        vehicle.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, VehicleStatus status)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == id && !v.IsDeleted);

        if (vehicle == null) return false;

        vehicle.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<VehicleDto>> GetByProviderAsync(Guid providerId)
    {
        return await _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.Provider)
            .Where(v => v.ProviderId == providerId && !v.IsDeleted)
            .OrderBy(v => v.Name)
            .Select(v => new VehicleDto
            {
                VehicleId = v.VehicleId,
                Name = v.Name,
                LicensePlate = v.LicensePlate,
                Capacity = v.Capacity,
                VehicleTypeId = v.VehicleTypeId,
                VehicleTypeName = v.VehicleType != null ? v.VehicleType.Name : "",
                ProviderId = v.ProviderId,
                ProviderName = v.Provider != null ? v.Provider.Name : "",
                Description = v.Description,
                ImageUrl = v.ImageUrl,
                Amenities = string.IsNullOrEmpty(v.Amenities) ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v.Amenities, new System.Text.Json.JsonSerializerOptions()),
                Status = v.Status.ToString(),
                ScheduleCount = v.Schedules.Count(s => !s.IsDeleted),
                CreatedAt = v.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<VehicleDto>> GetByTypeAsync(Guid vehicleTypeId)
    {
        return await _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.Provider)
            .Where(v => v.VehicleTypeId == vehicleTypeId && !v.IsDeleted)
            .OrderBy(v => v.Name)
            .Select(v => new VehicleDto
            {
                VehicleId = v.VehicleId,
                Name = v.Name,
                LicensePlate = v.LicensePlate,
                Capacity = v.Capacity,
                VehicleTypeId = v.VehicleTypeId,
                VehicleTypeName = v.VehicleType != null ? v.VehicleType.Name : "",
                ProviderId = v.ProviderId,
                ProviderName = v.Provider != null ? v.Provider.Name : "",
                Description = v.Description,
                ImageUrl = v.ImageUrl,
                Amenities = string.IsNullOrEmpty(v.Amenities) ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v.Amenities, new System.Text.Json.JsonSerializerOptions()),
                Status = v.Status.ToString(),
                ScheduleCount = v.Schedules.Count(s => !s.IsDeleted),
                CreatedAt = v.CreatedAt
            })
            .ToListAsync();
    }
}

// ==================== Schedule Service ====================

public interface IScheduleService
{
    Task<List<ScheduleDto>> GetAllAsync();
    Task<ScheduleDto?> GetByIdAsync(Guid id);
    Task<ScheduleDto> CreateAsync(CreateScheduleRequest request);
    Task<ScheduleDto?> UpdateAsync(Guid id, UpdateScheduleRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<List<ScheduleDto>> GetByVehicleAsync(Guid vehicleId);
    Task<List<ScheduleDto>> GetByRouteAsync(Guid routeId);
}

public class ScheduleService : IScheduleService
{
    private readonly KarnelTravelsDbContext _context;

    public ScheduleService(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScheduleDto>> GetAllAsync()
    {
        return await _context.Schedules
            .Include(s => s.Vehicle).ThenInclude(v => v!.VehicleType)
            .Include(s => s.Vehicle).ThenInclude(v => v!.Provider)
            .Include(s => s.Route)
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .Select(s => new ScheduleDto
            {
                ScheduleId = s.ScheduleId,
                VehicleId = s.VehicleId,
                VehicleName = s.Vehicle != null ? s.Vehicle.Name : "",
                VehicleLicensePlate = s.Vehicle != null ? s.Vehicle.LicensePlate : "",
                RouteId = s.RouteId,
                RouteName = s.Route != null ? s.Route.RouteName ?? "" : "",
                DepartureLocation = s.Route != null ? s.Route.DepartureLocation : "",
                ArrivalLocation = s.Route != null ? s.Route.ArrivalLocation : "",
                DepartureTime = s.DepartureTime,
                ArrivalTime = s.ArrivalTime,
                Price = s.Price,
                AvailableSeats = s.AvailableSeats,
                TotalSeats = s.TotalSeats,
                OperatingDays = string.IsNullOrEmpty(s.OperatingDays) ? null : System.Text.Json.JsonSerializer.Deserialize<List<DayOfWeek>>(s.OperatingDays, new System.Text.Json.JsonSerializerOptions()),
                Notes = s.Notes,
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ScheduleDto?> GetByIdAsync(Guid id)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Vehicle).ThenInclude(v => v!.VehicleType)
            .Include(s => s.Vehicle).ThenInclude(v => v!.Provider)
            .Include(s => s.Route)
            .FirstOrDefaultAsync(s => s.ScheduleId == id && !s.IsDeleted);

        if (schedule == null) return null;

        return new ScheduleDto
        {
            ScheduleId = schedule.ScheduleId,
            VehicleId = schedule.VehicleId,
            VehicleName = schedule.Vehicle?.Name ?? "",
            VehicleLicensePlate = schedule.Vehicle?.LicensePlate ?? "",
            RouteId = schedule.RouteId,
            RouteName = schedule.Route?.RouteName ?? "",
            DepartureLocation = schedule.Route?.DepartureLocation ?? "",
            ArrivalLocation = schedule.Route?.ArrivalLocation ?? "",
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            Price = schedule.Price,
            AvailableSeats = schedule.AvailableSeats,
            TotalSeats = schedule.TotalSeats,
            OperatingDays = string.IsNullOrEmpty(schedule.OperatingDays) ? null : System.Text.Json.JsonSerializer.Deserialize<List<DayOfWeek>>(schedule.OperatingDays, new System.Text.Json.JsonSerializerOptions()),
            Notes = schedule.Notes,
            Status = schedule.Status.ToString(),
            CreatedAt = schedule.CreatedAt
        };
    }

    public async Task<ScheduleDto> CreateAsync(CreateScheduleRequest request)
    {
        var schedule = new Schedule
        {
            VehicleId = request.VehicleId,
            RouteId = request.RouteId,
            DepartureTime = request.DepartureTime,
            ArrivalTime = request.ArrivalTime,
            Price = request.Price,
            AvailableSeats = request.TotalSeats,
            TotalSeats = request.TotalSeats,
            OperatingDays = request.OperatingDays != null ? System.Text.Json.JsonSerializer.Serialize(request.OperatingDays) : null,
            Notes = request.Notes,
            Status = request.Status
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.Provider)
            .FirstOrDefaultAsync(v => v.VehicleId == schedule.VehicleId);
        var route = await _context.Routes.FindAsync(schedule.RouteId);

        return new ScheduleDto
        {
            ScheduleId = schedule.ScheduleId,
            VehicleId = schedule.VehicleId,
            VehicleName = vehicle?.Name ?? "",
            VehicleLicensePlate = vehicle?.LicensePlate ?? "",
            RouteId = schedule.RouteId,
            RouteName = route?.RouteName ?? $"{route?.DepartureLocation} - {route?.ArrivalLocation}",
            DepartureLocation = route?.DepartureLocation ?? "",
            ArrivalLocation = route?.ArrivalLocation ?? "",
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            Price = schedule.Price,
            AvailableSeats = schedule.AvailableSeats,
            TotalSeats = schedule.TotalSeats,
            OperatingDays = request.OperatingDays,
            Notes = schedule.Notes,
            Status = schedule.Status.ToString(),
            CreatedAt = schedule.CreatedAt
        };
    }

    public async Task<ScheduleDto?> UpdateAsync(Guid id, UpdateScheduleRequest request)
    {
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ScheduleId == id && !s.IsDeleted);

        if (schedule == null) return null;

        if (request.DepartureTime.HasValue)
            schedule.DepartureTime = request.DepartureTime.Value;
        if (request.ArrivalTime.HasValue)
            schedule.ArrivalTime = request.ArrivalTime;
        if (request.Price.HasValue)
            schedule.Price = request.Price.Value;
        if (request.AvailableSeats.HasValue)
            schedule.AvailableSeats = request.AvailableSeats.Value;
        if (request.TotalSeats.HasValue)
            schedule.TotalSeats = request.TotalSeats.Value;
        if (request.OperatingDays != null)
            schedule.OperatingDays = System.Text.Json.JsonSerializer.Serialize(request.OperatingDays);
        if (request.Notes != null)
            schedule.Notes = request.Notes;
        if (request.Status.HasValue)
            schedule.Status = request.Status.Value;

        await _context.SaveChangesAsync();

        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.Provider)
            .FirstOrDefaultAsync(v => v.VehicleId == schedule.VehicleId);
        var route = await _context.Routes.FindAsync(schedule.RouteId);

        return new ScheduleDto
        {
            ScheduleId = schedule.ScheduleId,
            VehicleId = schedule.VehicleId,
            VehicleName = vehicle?.Name ?? "",
            VehicleLicensePlate = vehicle?.LicensePlate ?? "",
            RouteId = schedule.RouteId,
            RouteName = route?.RouteName ?? $"{route?.DepartureLocation} - {route?.ArrivalLocation}",
            DepartureLocation = route?.DepartureLocation ?? "",
            ArrivalLocation = route?.ArrivalLocation ?? "",
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            Price = schedule.Price,
            AvailableSeats = schedule.AvailableSeats,
            TotalSeats = schedule.TotalSeats,
            OperatingDays = request.OperatingDays,
            Notes = schedule.Notes,
            Status = schedule.Status.ToString(),
            CreatedAt = schedule.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => s.ScheduleId == id && !s.IsDeleted);

        if (schedule == null) return false;

        schedule.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ScheduleDto>> GetByVehicleAsync(Guid vehicleId)
    {
        return await _context.Schedules
            .Include(s => s.Vehicle).ThenInclude(v => v!.VehicleType)
            .Include(s => s.Vehicle).ThenInclude(v => v!.Provider)
            .Include(s => s.Route)
            .Where(s => s.VehicleId == vehicleId && !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .Select(s => new ScheduleDto
            {
                ScheduleId = s.ScheduleId,
                VehicleId = s.VehicleId,
                VehicleName = s.Vehicle != null ? s.Vehicle.Name : "",
                VehicleLicensePlate = s.Vehicle != null ? s.Vehicle.LicensePlate : "",
                RouteId = s.RouteId,
                RouteName = s.Route != null ? s.Route.RouteName ?? "" : "",
                DepartureLocation = s.Route != null ? s.Route.DepartureLocation : "",
                ArrivalLocation = s.Route != null ? s.Route.ArrivalLocation : "",
                DepartureTime = s.DepartureTime,
                ArrivalTime = s.ArrivalTime,
                Price = s.Price,
                AvailableSeats = s.AvailableSeats,
                TotalSeats = s.TotalSeats,
                OperatingDays = string.IsNullOrEmpty(s.OperatingDays) ? null : System.Text.Json.JsonSerializer.Deserialize<List<DayOfWeek>>(s.OperatingDays, new System.Text.Json.JsonSerializerOptions()),
                Notes = s.Notes,
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<ScheduleDto>> GetByRouteAsync(Guid routeId)
    {
        return await _context.Schedules
            .Include(s => s.Vehicle).ThenInclude(v => v!.VehicleType)
            .Include(s => s.Vehicle).ThenInclude(v => v!.Provider)
            .Include(s => s.Route)
            .Where(s => s.RouteId == routeId && !s.IsDeleted)
            .OrderBy(s => s.DepartureTime)
            .Select(s => new ScheduleDto
            {
                ScheduleId = s.ScheduleId,
                VehicleId = s.VehicleId,
                VehicleName = s.Vehicle != null ? s.Vehicle.Name : "",
                VehicleLicensePlate = s.Vehicle != null ? s.Vehicle.LicensePlate : "",
                RouteId = s.RouteId,
                RouteName = s.Route != null ? s.Route.RouteName ?? "" : "",
                DepartureLocation = s.Route != null ? s.Route.DepartureLocation : "",
                ArrivalLocation = s.Route != null ? s.Route.ArrivalLocation : "",
                DepartureTime = s.DepartureTime,
                ArrivalTime = s.ArrivalTime,
                Price = s.Price,
                AvailableSeats = s.AvailableSeats,
                TotalSeats = s.TotalSeats,
                OperatingDays = string.IsNullOrEmpty(s.OperatingDays) ? null : System.Text.Json.JsonSerializer.Deserialize<List<DayOfWeek>>(s.OperatingDays, new System.Text.Json.JsonSerializerOptions()),
                Notes = s.Notes,
                Status = s.Status.ToString(),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();
    }
}
