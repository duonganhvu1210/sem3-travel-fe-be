using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class Contact : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(200)]
    public string? Subject { get; set; }

    public ContactRequestType RequestType { get; set; } = ContactRequestType.General;

    [MaxLength(100)]
    public string? ServiceType { get; set; } // Tour, Hotel, Resort, Transport

    public DateTime? ExpectedDate { get; set; }

    public int? ParticipantCount { get; set; }

    [Required]
    public string MessageContent { get; set; } = string.Empty;

    public int? Rating { get; set; } // 1-5 for feedback

    public ContactStatus Status { get; set; } = ContactStatus.Unread;

    [MaxLength(1000)]
    public string? ReplyMessage { get; set; }

    public DateTime? RepliedAt { get; set; }

    public Guid? UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}

public enum ContactRequestType
{
    General = 0,
    Booking = 1,
    Consulting = 2,
    Feedback = 3,
    Callback = 4
}

public enum ContactStatus
{
    Unread = 0,
    Read = 1,
    Replied = 2,
    Closed = 3
}
