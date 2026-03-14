using System.ComponentModel.DataAnnotations;

namespace KarnelTravels.API.Entities;

public class VehicleType : BaseEntity
{
    public Guid VehicleTypeId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty; // Xe khách, Limousine, Máy bay, Tàu hỏa

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Icon { get; set; } // Icon name for UI

    // Navigation properties
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
