using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class AccountActivity : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? IPAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }
}

public static class AccountActivityActions
{
    public const string Login = "Login";
    public const string Logout = "Logout";
    public const string Register = "Register";
    public const string ChangePassword = "Change Password";
    public const string UpdateProfile = "Update Profile";
    public const string UploadAvatar = "Upload Avatar";
    public const string AddAddress = "Add Address";
    public const string UpdateAddress = "Update Address";
    public const string DeleteAddress = "Delete Address";
    public const string VerifyEmail = "Verify Email";
    public const string RequestVerificationEmail = "Request Verification Email";
    public const string CreateBooking = "Create Booking";
    public const string CancelBooking = "Cancel Booking";
}
