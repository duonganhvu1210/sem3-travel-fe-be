using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Services;

public interface IUserService
{
    Task<PagedUserListResponse> GetUsersAsync(UserSearchRequest request);
    Task<UserListDto?> GetUserByIdAsync(Guid userId);
    Task<UserListDto> CreateUserAsync(CreateUserRequest request);
    Task<UserListDto?> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> UpdateUserStatusAsync(Guid userId, bool isLocked);
    Task<bool> ResetPasswordAsync(Guid userId, string newPassword);
    Task<List<UserBookingHistoryDto>> GetUserBookingsAsync(Guid userId);
}

public class UserService : IUserService
{
    private readonly KarnelTravelsDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(KarnelTravelsDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    /// <summary>
    /// Lấy danh sách người dùng với phân trang và tìm kiếm (F219)
    /// </summary>
    public async Task<PagedUserListResponse> GetUsersAsync(UserSearchRequest request)
    {
        var query = _context.Users.Where(u => !u.IsDeleted).AsQueryable();

        // Tìm kiếm theo từ khóa
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(searchTerm) ||
                u.FullName.ToLower().Contains(searchTerm) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
        }

        // Lọc theo role
        if (request.Role.HasValue)
        {
            query = query.Where(u => u.Role == request.Role.Value);
        }

        // Lọc theo trạng thái khóa
        if (request.IsLocked.HasValue)
        {
            query = query.Where(u => u.IsLocked == request.IsLocked.Value);
        }

        // Sắp xếp
        query = request.SortBy?.ToLower() switch
        {
            "email" => request.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "fullname" => request.SortDescending ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName),
            "role" => request.SortDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
            "islocked" => request.SortDescending ? query.OrderByDescending(u => u.IsLocked) : query.OrderBy(u => u.IsLocked),
            _ => request.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var users = await query
            .Select(u => new UserListDto
            {
                UserId = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                Role = u.Role,
                IsLocked = u.IsLocked,
                IsEmailVerified = u.IsEmailVerified,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedUserListResponse
        {
            Users = users,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Lấy thông tin user theo ID
    /// </summary>
    public async Task<UserListDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null) return null;

        return new UserListDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Avatar = user.Avatar,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Role = user.Role,
            IsLocked = user.IsLocked,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    /// <summary>
    /// Tạo mới user (F220)
    /// </summary>
    public async Task<UserListDto> CreateUserAsync(CreateUserRequest request)
    {
        // Kiểm tra email đã tồn tại chưa
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && !u.IsDeleted);

        if (existingUser != null)
        {
            throw new InvalidOperationException("Email đã được sử dụng");
        }

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Role = request.Role,
            IsEmailVerified = true, // Admin tạo user nên auto verify
            IsLocked = false,
            CreatedAt = DateTime.UtcNow
        };

        // Hash mật khẩu
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserListDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Avatar = user.Avatar,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Role = user.Role,
            IsLocked = user.IsLocked,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    /// <summary>
    /// Cập nhật thông tin user (F221)
    /// </summary>
    public async Task<UserListDto?> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return null;
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.DateOfBirth = request.DateOfBirth;
        user.Gender = request.Gender;

        if (request.Role.HasValue)
        {
            user.Role = request.Role.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new UserListDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Avatar = user.Avatar,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Role = user.Role,
            IsLocked = user.IsLocked,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    /// <summary>
    /// Xóa user (F222)
    /// </summary>
    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return false;
        }

        // Soft delete
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Cập nhật trạng thái user - Kích hoạt/Vô hiệu hóa (F225)
    /// </summary>
    public async Task<bool> UpdateUserStatusAsync(Guid userId, bool isLocked)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return false;
        }

        user.IsLocked = isLocked;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Reset mật khẩu user (F224)
    /// </summary>
    public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || user.IsDeleted)
        {
            return false;
        }

        // Hash mật khẩu mới
        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        // Reset refresh token để buộc đăng nhập lại
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Lấy danh sách booking của user (F226)
    /// </summary>
    public async Task<List<UserBookingHistoryDto>> GetUserBookingsAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.IsDeleted)
        {
            return new List<UserBookingHistoryDto>();
        }

        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId && !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new UserBookingHistoryDto
            {
                BookingId = b.Id,
                BookingCode = b.BookingCode,
                Type = b.Type,
                Status = b.Status,
                ServiceDate = b.ServiceDate,
                EndDate = b.EndDate,
                Quantity = b.Quantity,
                TotalAmount = b.TotalAmount,
                DiscountAmount = b.DiscountAmount,
                FinalAmount = b.FinalAmount,
                PaymentStatus = b.PaymentStatus,
                PaymentMethod = b.PaymentMethod,
                PaidAt = b.PaidAt,
                CreatedAt = b.CreatedAt,
                ItemName = b.TourPackage != null ? b.TourPackage.Name :
                           b.Hotel != null ? b.Hotel.Name :
                           b.Resort != null ? b.Resort.Name :
                           b.Transport != null ? b.Transport.Type :
                           b.Restaurant != null ? b.Restaurant.Name : "",
                ItemId = b.TourPackageId ?? b.HotelId ?? b.ResortId ?? b.TransportId ?? b.RestaurantId
            })
            .ToListAsync();

        return bookings;
    }
}
