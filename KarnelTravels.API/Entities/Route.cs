using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class Route : BaseEntity
{
    public Guid RouteId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string DepartureLocation { get; set; } = string.Empty; // e.g., "Hà Nội"

    [Required]
    [MaxLength(100)]
    public string ArrivalLocation { get; set; } = string.Empty; // e.g., "TP. Hồ Chí Minh"

    [MaxLength(200)]
    public string? RouteName { get; set; } // e.g., "Hà Nội - TP. Hồ Chí Minh"

    public double? Distance { get; set; } // F240: Distance in km

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? EstimatedDuration { get; set; } // e.g., "2 giờ 30 phút"

    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
