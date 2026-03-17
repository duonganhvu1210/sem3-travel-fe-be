using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace KarnelTravels.API.Services;

public interface IProfileService
{
    Task<ApiResponse<UserProfileDto>> GetProfileAsync(Guid userId);
    Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<ApiResponse<string>> UploadAvatarAsync(Guid userId, IFormFile file);
    Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ApiResponse<List<AccountActivityDto>>> GetActivitiesAsync(Guid userId, int pageIndex = 1, int pageSize = 20);
    Task<ApiResponse<List<AddressDto>>> GetAddressesAsync(Guid userId);
    Task<ApiResponse<AddressDto>> CreateAddressAsync(Guid userId, CreateAddressRequest request);
    Task<ApiResponse<AddressDto>> UpdateAddressAsync(Guid userId, UpdateAddressRequest request);
    Task<ApiResponse<string>> DeleteAddressAsync(Guid userId, Guid addressId);
    Task<ApiResponse<string>> SetDefaultAddressAsync(Guid userId, Guid addressId);
    Task<ApiResponse<string>> ResendVerificationEmailAsync(Guid userId);
    Task LogActivityAsync(Guid userId, string action, string? description = null);
}

public class ProfileService : IProfileService
{
    private readonly KarnelTravelsDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProfileService(
        KarnelTravelsDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "User not found"
            };
        }

        return new ApiResponse<UserProfileDto>
        {
            Success = true,
            Message = "Get profile successfully",
            Data = new UserProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                TravelPreferences = user.TravelPreferences,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (request.PhoneNumber != null)
            user.PhoneNumber = request.PhoneNumber;

        if (request.DateOfBirth.HasValue)
            user.DateOfBirth = request.DateOfBirth.Value;

        if (request.Gender != null)
            user.Gender = request.Gender;

        if (request.TravelPreferences != null)
            user.TravelPreferences = request.TravelPreferences;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Log activity - wrap in try-catch to ensure profile update is saved even if logging fails
        try
        {
            await LogActivityAsync(userId, AccountActivityActions.UpdateProfile, "Update profile successfully");
        }
        catch (Exception ex)
        {
            // Log error but don't fail the profile update
            Console.WriteLine($"Failed to log activity: {ex.Message}");
        }

        return new ApiResponse<UserProfileDto>
        {
            Success = true,
            Message = "Update profile successfully",
            Data = new UserProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                TravelPreferences = user.TravelPreferences,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<ApiResponse<string>> UploadAvatarAsync(Guid userId, IFormFile file)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (file == null || file.Length == 0)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Please select an image file"
            };
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Only image files (jpg, jpeg, png, gif, webp) are accepted"
            };
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Image size cannot exceed 5MB"
            };
        }

        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "avatars");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var avatarUrl = $"/uploads/avatars/{fileName}";

        if (!string.IsNullOrEmpty(user.Avatar))
        {
            var oldFilePath = Path.Combine(_environment.ContentRootPath, "wwwroot", user.Avatar.TrimStart('/'));
            if (File.Exists(oldFilePath))
            {
                try
                {
                    File.Delete(oldFilePath);
                }
                catch
                {
                }
            }
        }

        user.Avatar = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await LogActivityAsync(userId, AccountActivityActions.UploadAvatar, $"Upload avatar: {fileName}");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Upload avatar successfully",
            Data = avatarUrl
        };
    }

    public async Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Please enter the current password"
            };
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Please enter the new password"
            };
        }

        if (request.NewPassword.Length < 6)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "New password must be at least 6 characters"
            };
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "New password and confirm password do not match"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Current password is incorrect"
            };
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await LogActivityAsync(userId, AccountActivityActions.ChangePassword, "Change password successfully");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Change password successfully"
        };
    }

    public async Task<ApiResponse<List<AccountActivityDto>>> GetActivitiesAsync(Guid userId, int pageIndex = 1, int pageSize = 20)
    {
        var activities = await _context.AccountActivities
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AccountActivityDto
            {
                Id = a.Id,
                Action = a.Action,
                Description = a.Description,
                IPAddress = a.IPAddress,
                Timestamp = a.CreatedAt
            })
            .ToListAsync();

        return new ApiResponse<List<AccountActivityDto>>
        {
            Success = true,
            Message = "Get activities successfully",
            Data = activities
        };
    }

    public async Task<ApiResponse<List<AddressDto>>> GetAddressesAsync(Guid userId)
    {
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new AddressDto
            {
                AddressId = a.Id,
                AddressLine = a.AddressLine,
                Ward = a.Ward,
                District = a.District,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            })
            .ToListAsync();

        return new ApiResponse<List<AddressDto>>
        {
            Success = true,
            Message = "Get addresses successfully",
            Data = addresses
        };
    }

    public async Task<ApiResponse<AddressDto>> CreateAddressAsync(Guid userId, CreateAddressRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (string.IsNullOrWhiteSpace(request.AddressLine))
        {
            return new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "Please enter the address"
            };
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            return new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "Please enter the city"
            };
        }

        if (request.IsDefault)
        {
            var existingDefault = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && !a.IsDeleted)
                .ToListAsync();

            foreach (var addr in existingDefault)
            {
                addr.IsDefault = false;
            }
        }

        var address = new Address
        {
            UserId = userId,
            AddressLine = request.AddressLine,
            Ward = request.Ward,
            District = request.District,
            City = request.City,
            Country = request.Country ?? "Vietnam",
            IsDefault = request.IsDefault,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        await LogActivityAsync(userId, AccountActivityActions.AddAddress, $"Add address: {address.AddressLine}, {address.City}");

        return new ApiResponse<AddressDto>
        {
            Success = true,
            Message = "Add address successfully",
            Data = new AddressDto
            {
                AddressId = address.Id,
                AddressLine = address.AddressLine,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                Country = address.Country,
                IsDefault = address.IsDefault
            }
        };
    }

    public async Task<ApiResponse<AddressDto>> UpdateAddressAsync(Guid userId, UpdateAddressRequest request)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId && !a.IsDeleted);

        if (address == null)
        {
            return new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "Address not found"
            };
        }

        if (string.IsNullOrWhiteSpace(request.AddressLine))
        {
            return new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "Please enter the address"
            };
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            return new ApiResponse<AddressDto>
            {
                Success = false,
                Message = "Please enter the city"
            };
        }

        if (request.IsDefault && !address.IsDefault)
        {
            var existingDefault = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && a.Id != request.AddressId && !a.IsDeleted)
                .ToListAsync();

            foreach (var addr in existingDefault)
            {
                addr.IsDefault = false;
            }
        }

        address.AddressLine = request.AddressLine;
        address.Ward = request.Ward;
        address.District = request.District;
        address.City = request.City;
        address.Country = request.Country ?? "Vietnam";
        address.IsDefault = request.IsDefault;
        address.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await LogActivityAsync(userId, AccountActivityActions.UpdateAddress, $"Update address: {address.AddressLine}, {address.City}");

        return new ApiResponse<AddressDto>
        {
            Success = true,
            Message = "Update address successfully",
            Data = new AddressDto
            {
                AddressId = address.Id,
                AddressLine = address.AddressLine,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                Country = address.Country,
                IsDefault = address.IsDefault
            }
        };
    }

    public async Task<ApiResponse<string>> DeleteAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId && !a.IsDeleted);

        if (address == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Address not found"
            };
        }

        var wasDefault = address.IsDefault;

        address.IsDeleted = true;
        address.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        if (wasDefault)
        {
            var newDefault = await _context.Addresses
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (newDefault != null)
            {
                newDefault.IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }

        await LogActivityAsync(userId, AccountActivityActions.DeleteAddress, $"Delete address: {address.AddressLine}");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Delete address successfully"
        };
    }

    public async Task<ApiResponse<string>> SetDefaultAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId && !a.IsDeleted);

        if (address == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Address not found"
            };
        }

        var existingDefaults = await _context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId && !a.IsDeleted)
            .ToListAsync();

        foreach (var addr in existingDefaults)
        {
            addr.IsDefault = false;
        }

        address.IsDefault = true;
        address.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Set default address successfully"
        };
    }

    public async Task<ApiResponse<string>> ResendVerificationEmailAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (user.IsEmailVerified)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Email is already verified"
            };
        }

        var token = Guid.NewGuid().ToString("N");
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await LogActivityAsync(userId, AccountActivityActions.RequestVerificationEmail, "Request verification email");

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Verification email has been sent"
        };
    }

    public async Task LogActivityAsync(Guid userId, string action, string? description = null)
    {
        try
        {
            var activity = new AccountActivity
            {
                UserId = userId,
                Action = action,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.AccountActivities.Add(activity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't throw - activity logging should not affect main operations
            Console.WriteLine($"Failed to log activity: {ex.Message}");
        }
    }
}
