using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace KarnelTravels.API.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ApiResponse<AuthResponse>> GetCurrentUserAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly KarnelTravelsDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(KarnelTravelsDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);

        if (user == null)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Email hoặc mật khẩu không đúng"
            };
        }

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Email hoặc mật khẩu không đúng"
            };
        }

        // Check if account is locked
        if (user.IsLocked)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên"
            };
        }

        // Generate tokens
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Đăng nhập thành công",
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Avatar = user.Avatar,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600 // 1 hour
            }
        };
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);

        if (existingUser != null)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Email đã được sử dụng"
            };
        }

        // Create new user
        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User, // Default role
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate tokens
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Đăng ký thành công",
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Avatar = user.Avatar,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600
            }
        };
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted);

        if (user == null)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Refresh token không hợp lệ"
            };
        }

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Refresh token đã hết hạn"
            };
        }

        // Generate new tokens
        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Token đã được làm mới",
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Avatar = user.Avatar,
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 3600
            }
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
                Message = "Không tìm thấy người dùng"
            };
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Mật khẩu hiện tại không đúng"
            };
        }

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Đổi mật khẩu thành công"
        };
    }

    public async Task<ApiResponse<AuthResponse>> GetCurrentUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Không tìm thấy người dùng"
            };
        }

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Avatar = user.Avatar
            }
        };
    }

    #region Private Methods

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "KarnelTravelsSecretKey2026VeryLong!@#$%^&*()"));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("role", user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "KarnelTravels",
            audience: _configuration["Jwt:Audience"] ?? "KarnelTravels",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }

    #endregion
}
