namespace KarnelTravels.API.DTOs;

using System.ComponentModel.DataAnnotations;
using KarnelTravels.API.Entities;

// TouristSpot DTOs
public class TouristSpotDto
{
    public Guid SpotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string>? Images { get; set; }
    public decimal? TicketPrice { get; set; }
    public string? BestTime { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTouristSpotRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string>? Images { get; set; }
    public decimal? TicketPrice { get; set; }
    public string? BestTime { get; set; }
    public bool IsFeatured { get; set; } = false;
}

public class ImageUploadResult
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}

// Hotel DTOs
public class HotelDto
{
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string>? Images { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string>? Amenities { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsFeatured { get; set; }
}

public class HotelRoomDto
{
    public Guid RoomId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxOccupancy { get; set; }
    public decimal PricePerNight { get; set; }
    public string? BedType { get; set; }
    public List<string>? RoomAmenities { get; set; }
    public List<string>? Images { get; set; }
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public bool IsAvailable { get; set; }
}

public class HotelReviewDto
{
    public Guid ReviewId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HotelDetailDto
{
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? CancellationPolicy { get; set; }
    public List<string>? Images { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string>? Amenities { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsFeatured { get; set; }
    public List<HotelRoomDto>? Rooms { get; set; }
    public List<HotelReviewDto>? Reviews { get; set; }
}

public class HotelFilterOptionsDto
{
    public List<string> Cities { get; set; } = new();
    public List<int> StarRatings { get; set; } = new();
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public List<string> Amenities { get; set; } = new();
}

public class CreateHotelRoomRequest
{
    public string RoomType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxOccupancy { get; set; } = 2;
    public decimal PricePerNight { get; set; }
    public string? BedType { get; set; }
    public List<string>? RoomAmenities { get; set; }
    public List<string>? Images { get; set; }
    public int TotalRooms { get; set; }
}

public class CreateHotelRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public int StarRating { get; set; } = 3;
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public List<string>? Images { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string>? Amenities { get; set; }
    public string? CancellationPolicy { get; set; }
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public bool IsFeatured { get; set; } = false;
    public List<CreateHotelRoomRequest>? Rooms { get; set; }
}

// Transport DTOs
public class TransportDto
{
    public Guid TransportId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? DepartureTime { get; set; }
    public string? ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? Images { get; set; }
    public bool IsFeatured { get; set; }
}

public class TransportStatisticsDto
{
    public int TotalTransports { get; set; }
    public int AverageDurationMinutes { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public List<string> AvailableTypes { get; set; } = new();
    public List<string> AvailableProviders { get; set; } = new();
    public List<string> AvailableRoutes { get; set; } = new();
    public Dictionary<string, int> AverageDurationByType { get; set; } = new();
}

public class TransportFilterOptionsDto
{
    public List<string> Types { get; set; } = new();
    public List<string> Providers { get; set; } = new();
    public List<string> Cities { get; set; } = new();
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int MinDuration { get; set; }
    public int MaxDuration { get; set; }
}

public class CreateTransportRequest
{
    public string Type { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? DepartureTime { get; set; }
    public string? ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? Images { get; set; }
    public bool IsFeatured { get; set; } = false;
}

// Tour Package DTOs
public class TourPackageDto
{
    public Guid TourId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Destination { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public List<string>? Images { get; set; }
    public List<TourItineraryDto>? Itineraries { get; set; }
    public List<string>? Includes { get; set; }
    public List<string>? Excludes { get; set; }
    public int AvailableSlots { get; set; }
    public List<string>? DepartureDates { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsHotDeal { get; set; }
}

// Booking DTOs
public class BookingDto
{
    public Guid BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ItemName { get; set; } = string.Empty;
}

public class CreateBookingRequest
{
    public string Type { get; set; } = string.Empty;
    public Guid? TourPackageId { get; set; }
    public Guid? HotelId { get; set; }
    public Guid? HotelRoomId { get; set; }
    public Guid? ResortId { get; set; }
    public Guid? ResortRoomId { get; set; }
    public Guid? TransportId { get; set; }
    public Guid? RestaurantId { get; set; }
    public DateTime? ServiceDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Quantity { get; set; } = 1;
    public string? PromoCode { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

// Promotion DTOs
public class PromotionDto
{
    public Guid PromoId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string>? ApplicableTo { get; set; }
    public bool IsActive { get; set; }
    public bool ShowOnHome { get; set; }
}

public class CreatePromotionRequest
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string>? ApplicableTo { get; set; }
    public bool ShowOnHome { get; set; } = false;
}

// Contact DTOs
public class ContactDto
{
    public Guid ContactId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Subject { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string? ServiceType { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public int? ParticipantCount { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateContactRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Subject { get; set; }
    public string? RequestType { get; set; }
    public string? ServiceType { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public int? ParticipantCount { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public int? Rating { get; set; }
}

public class UpdateContactStatusRequest
{
    public ContactStatus Status { get; set; }
    public string? ReplyMessage { get; set; }
}

public class UpdateContactRequest
{
    public string? ReplyMessage { get; set; }
    public ContactStatus Status { get; set; }
}

// Review DTOs
public class ReviewDto
{
    public Guid ReviewId { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? Images { get; set; }
    public string Type { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? Images { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid? TouristSpotId { get; set; }
    public Guid? HotelId { get; set; }
    public Guid? RestaurantId { get; set; }
    public Guid? ResortId { get; set; }
    public Guid? TourPackageId { get; set; }
    public Guid? BookingId { get; set; }
}

// Favorite DTOs
public class FavoriteDto
{
    public Guid FavoriteId { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemImage { get; set; }
    public decimal? ItemPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateFavoriteRequest
{
    public string ItemType { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
}

// ==================== Vehicle Management DTOs ====================

// VehicleType DTOs
public class VehicleTypeDto
{
    public Guid VehicleTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
}

// TransportProvider DTOs
public class TransportProviderDto
{
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public int VehicleCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTransportProviderRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
}

// Route DTOs
public class RouteDto
{
    public Guid RouteId { get; set; }
    public string DepartureLocation { get; set; } = string.Empty;
    public string ArrivalLocation { get; set; } = string.Empty;
    public string? RouteName { get; set; }
    public double? Distance { get; set; }
    public string? Description { get; set; }
    public string? EstimatedDuration { get; set; }
    public int ScheduleCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRouteRequest
{
    public string DepartureLocation { get; set; } = string.Empty;
    public string ArrivalLocation { get; set; } = string.Empty;
    public string? RouteName { get; set; }
    public double? Distance { get; set; }
    public string? Description { get; set; }
    public string? EstimatedDuration { get; set; }
}

// Vehicle DTOs
public class VehicleDto
{
    public Guid VehicleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid VehicleTypeId { get; set; }
    public string VehicleTypeName { get; set; } = string.Empty;
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Amenities { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ScheduleCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleRequest
{
    public string Name { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public Guid VehicleTypeId { get; set; }
    public Guid ProviderId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Amenities { get; set; }
    public string Status { get; set; } = "Available";
}

// Schedule DTOs
public class ScheduleDto
{
    public Guid ScheduleId { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public Guid RouteId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public string DepartureLocation { get; set; } = string.Empty;
    public string ArrivalLocation { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan? ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
    public List<DayOfWeek>? OperatingDays { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateScheduleRequest
{
    public Guid VehicleId { get; set; }
    public Guid RouteId { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan? ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public int TotalSeats { get; set; }
    public List<DayOfWeek>? OperatingDays { get; set; }
    public string? Notes { get; set; }
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Active;
}

public class UpdateScheduleRequest
{
    public TimeSpan? DepartureTime { get; set; }
    public TimeSpan? ArrivalTime { get; set; }
    public decimal? Price { get; set; }
    public int? AvailableSeats { get; set; }
    public int? TotalSeats { get; set; }
    public List<DayOfWeek>? OperatingDays { get; set; }
    public string? Notes { get; set; }
    public ScheduleStatus? Status { get; set; }
}

public class UpdateVehicleStatusRequest
{
    public VehicleStatus Status { get; set; }
}

// ==================== Tour Management DTOs ====================

// Tour DTOs
public class TourDto
{
    public Guid TourId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationDays { get; set; }
    public int DurationNights { get; set; }
    public decimal BasePrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public List<string>? Highlights { get; set; }
    public string? Terms { get; set; }
    public string? CancellationPolicy { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsDomestic { get; set; }
    public int BookingCount { get; set; }
    public int TotalDepartures { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TourWithDetailsDto : TourDto
{
    public List<TourItineraryDto> Itineraries { get; set; } = new();
    public List<TourDestinationDto> Destinations { get; set; } = new();
    public List<TourDepartureDto> Departures { get; set; } = new();
    public List<TourServiceDto> Services { get; set; } = new();
    public List<TourGuideDto> TourGuides { get; set; } = new();
    public List<TourImageDto> Images { get; set; } = new();
}

public class CreateTourRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(3000)]
    public string? Description { get; set; }

    public int DurationDays { get; set; }
    public int DurationNights { get; set; }
    public decimal BasePrice { get; set; }
    public TourStatus Status { get; set; } = TourStatus.Draft;
    public string? ThumbnailUrl { get; set; }
    public List<string>? Highlights { get; set; }
    public string? Terms { get; set; }
    public string? CancellationPolicy { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsDomestic { get; set; } = true;
}

public class UpdateTourRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? DurationDays { get; set; }
    public int? DurationNights { get; set; }
    public decimal? BasePrice { get; set; }
    public TourStatus? Status { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<string>? Highlights { get; set; }
    public string? Terms { get; set; }
    public string? CancellationPolicy { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsDomestic { get; set; }
}

// Tour Itinerary DTOs
public class TourItineraryDto
{
    public Guid ItineraryId { get; set; }
    public int DayNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public List<string>? Meals { get; set; }
    public string? Accommodation { get; set; }
    public string? Transport { get; set; }
    public List<string>? Activities { get; set; }
    public string? Notes { get; set; }
}

public class CreateTourItineraryRequest
{
    public int DayNumber { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(5000)]
    public string? Content { get; set; }

    public List<string>? Meals { get; set; }
    public string? Accommodation { get; set; }
    public string? Transport { get; set; }
    public List<string>? Activities { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTourItineraryRequest
{
    public int? DayNumber { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? Meals { get; set; }
    public string? Accommodation { get; set; }
    public string? Transport { get; set; }
    public List<string>? Activities { get; set; }
    public string? Notes { get; set; }
}

// Tour Destination DTOs
public class TourDestinationDto
{
    public Guid TourDestinationId { get; set; }
    public Guid TouristSpotId { get; set; }
    public string TouristSpotName { get; set; } = string.Empty;
    public string? TouristSpotImage { get; set; }
    public int DisplayOrder { get; set; }
}

public class AddTourDestinationRequest
{
    public Guid TouristSpotId { get; set; }
    public int DisplayOrder { get; set; }
}

// Tour Departure DTOs
public class TourDepartureDto
{
    public Guid DepartureId { get; set; }
    public DateTime DepartureDate { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public bool IsAvailable { get; set; }
    public string? Notes { get; set; }
    public int BookedSeats { get; set; }
}

public class CreateTourDepartureRequest
{
    public DateTime DepartureDate { get; set; }
    public int TotalSeats { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTourDepartureRequest
{
    public DateTime? DepartureDate { get; set; }
    public int? TotalSeats { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public bool? IsAvailable { get; set; }
    public string? Notes { get; set; }
}

public class BulkCreateDepartureRequest
{
    public List<DateTime> Dates { get; set; } = new();
    public int TotalSeats { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? Notes { get; set; }
}

// Tour Service DTOs
public class TourServiceDto
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsIncluded { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class CreateTourServiceRequest
{
    [Required]
    [MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsIncluded { get; set; } = true;
    public ServiceCategory Category { get; set; } = ServiceCategory.Other;
}

public class UpdateTourServiceRequest
{
    public string? ServiceName { get; set; }
    public string? Description { get; set; }
    public bool? IsIncluded { get; set; }
    public ServiceCategory? Category { get; set; }
}

// Tour Guide DTOs
public class TourGuideDto
{
    public Guid GuideId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public List<string>? Specialties { get; set; }
    public int YearsExperience { get; set; }
    public bool IsAvailable { get; set; }
}

public class CreateTourGuideRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    public List<string>? Specialties { get; set; }
    public int YearsExperience { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class UpdateTourGuideRequest
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public List<string>? Specialties { get; set; }
    public int? YearsExperience { get; set; }
    public bool? IsAvailable { get; set; }
}

// Tour Image DTOs
public class TourImageDto
{
    public Guid ImageId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}

public class AddTourImageRequest
{
    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Caption { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}

public class UpdateTourImageRequest
{
    public string? ImageUrl { get; set; }
    public string? Caption { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsPrimary { get; set; }
}

// ==================== Restaurant Management DTOs ====================

// Restaurant with Menu and Reviews
public class RestaurantDto
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public string CuisineType { get; set; } = string.Empty;
    public string PriceLevel { get; set; } = "Budget";
    public string Style { get; set; } = "Restaurant";
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
    public string? ContactPhone { get; set; }
    public List<string>? Images { get; set; }
    public List<MenuItemDto>? Menu { get; set; }
    public bool HasReservation { get; set; } = true;
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOpen => IsCurrentlyOpen();
    
    private bool IsCurrentlyOpen()
    {
        if (string.IsNullOrEmpty(OpeningTime) || string.IsNullOrEmpty(ClosingTime))
            return false;
            
        if (!TimeSpan.TryParse(OpeningTime, out var openTime) || 
            !TimeSpan.TryParse(ClosingTime, out var closeTime))
            return false;
            
        var now = DateTime.Now.TimeOfDay;
        return now >= openTime && now <= closeTime;
    }
}

public class CreateRestaurantRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public CuisineType CuisineType { get; set; } = CuisineType.Vietnamese;
    public PriceRange PriceLevel { get; set; } = PriceRange.Budget;
    public RestaurantStyle Style { get; set; } = RestaurantStyle.Restaurant;
    
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
    
    [MaxLength(50)]
    public string? ContactPhone { get; set; }
    
    public List<string>? Images { get; set; }
    public List<MenuItemDto>? Menu { get; set; }
    public bool HasReservation { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
}

public class UpdateRestaurantRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public CuisineType? CuisineType { get; set; }
    public PriceRange? PriceLevel { get; set; }
    public RestaurantStyle? Style { get; set; }
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
    public string? ContactPhone { get; set; }
    public List<string>? Images { get; set; }
    public bool? HasReservation { get; set; }
    public bool? IsFeatured { get; set; }
}

public class UpdateRestaurantHoursRequest
{
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
}

// Menu Item DTOs
public class MenuItemDto
{
    public Guid MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = "Main"; // Appetizer, Main, Dessert, Drink
    public bool IsAvailable { get; set; } = true;
    public bool IsVegetarian { get; set; } = false;
    public bool IsSpicy { get; set; } = false;
}

public class CreateMenuItemRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }
    public string Category { get; set; } = "Main";
    public bool IsAvailable { get; set; } = true;
    public bool IsVegetarian { get; set; } = false;
    public bool IsSpicy { get; set; } = false;
}

public class UpdateMenuItemRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? IsVegetarian { get; set; }
    public bool? IsSpicy { get; set; }
}

// Restaurant Table Reservation DTOs
public class TableReservationDto
{
    public Guid ReservationId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int PartySize { get; set; }
    public DateTime ReservationDate { get; set; }
    public string ReservationTime { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
    public DateTime CreatedAt { get; set; }
}

public class CreateTableReservationRequest
{
    [Required]
    public Guid RestaurantId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required]
    [Range(1, 50)]
    public int PartySize { get; set; }

    [Required]
    public DateTime ReservationDate { get; set; }

    [Required]
    public string ReservationTime { get; set; } = string.Empty;

    public string? Notes { get; set; }
}

public class UpdateReservationStatusRequest
{
    public string Status { get; set; } = "Pending";
}

// ==================== Resort Management DTOs ====================

public class ResortDto
{
    public Guid ResortId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public List<string>? Images { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? Activities { get; set; }
    public List<ResortServiceDto>? Services { get; set; }
    public List<ComboPackageDto>? Packages { get; set; }
    public List<ResortRoomDto>? Rooms { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateResortRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public ResortType LocationType { get; set; } = ResortType.Beach;
    public int StarRating { get; set; } = 3;
    public List<string>? Images { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<ResortAmenity>? Amenities { get; set; }
    public bool IsFeatured { get; set; } = false;
}

public class UpdateResortRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public ResortType? LocationType { get; set; }
    public int? StarRating { get; set; }
    public List<string>? Images { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<ResortAmenity>? Amenities { get; set; }
    public bool? IsFeatured { get; set; }
}

// Resort Room DTOs
public class ResortRoomDto
{
    public Guid RoomId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaxOccupancy { get; set; }
    public decimal PricePerNight { get; set; }
    public string? BedType { get; set; }
    public List<string>? RoomAmenities { get; set; }
    public List<string>? Images { get; set; }
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public bool IsAvailable { get; set; }
}

public class CreateResortRoomRequest
{
    [Required]
    [MaxLength(100)]
    public string RoomType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(1, 20)]
    public int MaxOccupancy { get; set; } = 2;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal PricePerNight { get; set; }

    public string? BedType { get; set; }
    public List<string>? RoomAmenities { get; set; }
    public List<string>? Images { get; set; }

    [Range(1, 1000)]
    public int TotalRooms { get; set; }
}

public class UpdateResortRoomRequest
{
    public string? RoomType { get; set; }
    public string? Description { get; set; }
    public int? MaxOccupancy { get; set; }
    public decimal? PricePerNight { get; set; }
    public string? BedType { get; set; }
    public List<string>? RoomAmenities { get; set; }
    public List<string>? Images { get; set; }
    public int? TotalRooms { get; set; }
    public int? AvailableRooms { get; set; }
}

public class UpdateRoomAvailabilityRequest
{
    public int AvailableRooms { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal? PriceOverride { get; set; }
    public List<RoomAvailabilityUpdateItem> Availabilities { get; set; } = new();
}

public class RoomAvailabilityUpdateItem
{
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public decimal Price { get; set; }
}

// Resort Service DTOs
public class ResortServiceDto
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public ResortService Category { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Schedule { get; set; }
}

public class CreateResortServiceRequest
{
    [Required]
    [MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public ResortService Category { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Schedule { get; set; }
}

public class UpdateResortServiceRequest
{
    public string? ServiceName { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public ResortService? Category { get; set; }
    public bool? IsAvailable { get; set; }
    public string? Schedule { get; set; }
}

// Combo Package DTOs
public class ComboPackageDto
{
    public Guid PackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? RoomType { get; set; }
    public bool IncludesMeals { get; set; }
    public int MealPlan { get; set; } // 0: None, 1: Breakfast, 2: HalfBoard, 3: FullBoard
    public List<string>? IncludedServices { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class CreateComboPackageRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    public decimal? DiscountPrice { get; set; }
    public string? RoomType { get; set; }
    public bool IncludesMeals { get; set; }
    public int MealPlan { get; set; }
    public List<string>? IncludedServices { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateComboPackageRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? RoomType { get; set; }
    public bool? IncludesMeals { get; set; }
    public int? MealPlan { get; set; }
    public List<string>? IncludedServices { get; set; }
    public bool? IsAvailable { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

// Room Availability DTOs
public class RoomAvailabilityDto
{
    public Guid RoomId { get; set; }
    public DateTime Date { get; set; }
    public int AvailableRooms { get; set; }
    public decimal? PriceOverride { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsHoliday { get; set; }
}

public class BulkUpdateRoomAvailabilityRequest
{
    public List<RoomAvailabilityDto> Availabilities { get; set; } = new();
}

public class RoomPricingDto
{
    public Guid RoomId { get; set; }
    public DateTime Date { get; set; }
    public decimal BasePrice { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsAvailable { get; set; }
}
