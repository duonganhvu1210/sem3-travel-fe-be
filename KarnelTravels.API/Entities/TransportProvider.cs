using System.ComponentModel.DataAnnotations;

namespace KarnelTravels.API.Entities;

public class TransportProvider : BaseEntity
{
    public Guid ProviderId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty; // Vietnam Airlines, Mai Linh, etc.

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    [MaxLength(100)]
    public string? ContactEmail { get; set; }

    [MaxLength(200)]
    public string? ContactAddress { get; set; }

    [MaxLength(100)]
    public string? Website { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    // Navigation properties
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
