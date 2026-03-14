using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class Vehicle : BaseEntity
{
    public Guid VehicleId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty; // Name of the vehicle

    [Required]
    [MaxLength(50)]
    public string LicensePlate { get; set; } = string.Empty; // Biển số xe

    [Required]
    public int Capacity { get; set; } // Số ghế

    [Required]
    public Guid VehicleTypeId { get; set; }

    [ForeignKey(nameof(VehicleTypeId))]
    public virtual VehicleType? VehicleType { get; set; }

    [Required]
    public Guid ProviderId { get; set; }

    [ForeignKey(nameof(ProviderId))]
    public virtual TransportProvider? Provider { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; } // F243: Image upload

    [MaxLength(100)]
    public string? Amenities { get; set; } // JSON array: WiFi, AC, Water, etc.

    // F244: Status
    [Required]
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;

    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}

public enum VehicleStatus
{
    Available = 0,
    InService = 1,
    Maintenance = 2,
    Retired = 3
}
