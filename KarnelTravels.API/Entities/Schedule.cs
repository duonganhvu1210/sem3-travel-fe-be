using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class Schedule : BaseEntity
{
    public Guid ScheduleId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid VehicleId { get; set; }

    [ForeignKey(nameof(VehicleId))]
    public virtual Vehicle? Vehicle { get; set; }

    [Required]
    public Guid RouteId { get; set; }

    [ForeignKey(nameof(RouteId))]
    public virtual Route? Route { get; set; }

    [Required]
    public TimeSpan DepartureTime { get; set; } // F241: Giờ khởi hành

    public TimeSpan? ArrivalTime { get; set; } // F241: Giờ đến

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } // F242: Giá vé

    public int AvailableSeats { get; set; }

    public int TotalSeats { get; set; }

    public string? OperatingDays { get; set; } // Ngày hoạt động trong tuần (JSON)

    [MaxLength(100)]
    public string? Notes { get; set; }

    // F244: Status
    [Required]
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Active;
}

public enum ScheduleStatus
{
    Active = 0,
    Inactive = 1,
    Cancelled = 2
}
