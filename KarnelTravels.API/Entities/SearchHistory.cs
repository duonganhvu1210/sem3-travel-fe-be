using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class SearchHistory : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [MaxLength(200)]
    public string SearchQuery { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // spots, hotels, tours, etc.

    public DateTime SearchDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? Location { get; set; }

    public int ResultCount { get; set; } = 0;

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}
