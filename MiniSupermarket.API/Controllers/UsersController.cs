using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly SupermarketDbContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public UsersController(SupermarketDbContext db)
        {
            _db = db;
        }

        // =====================================================
        // GET: api/users
        // Lấy danh sách tài khoản
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .OrderBy(u => u.UserId)
                .Select(u => new UserDto
                {
                    Id = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        // =====================================================
        // POST: api/users
        // Tạo tài khoản mới
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest(new { message = "Tên đăng nhập không được để trống." });

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Mật khẩu không được để trống." });

            if (string.IsNullOrWhiteSpace(request.FullName))
                return BadRequest(new { message = "Họ và tên không được để trống." });

            if (request.Password.Length < 6)
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự." });

            if (!new[] { "Admin", "Cashier", "Warehouse" }.Contains(request.Role))
                return BadRequest(new { message = "Vai trò không hợp lệ." });
            string username = request.Username.Trim();

            bool exists = await _db.Users
                .AnyAsync(u => u.Username.ToLower() == username.ToLower());

            if (exists)
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại." });

            var user = new User
            {
                Username = username,
                FullName = request.FullName.Trim(),
                Role = string.IsNullOrWhiteSpace(request.Role)
                    ? "Cashier"
                    : request.Role.Trim(),
                IsActive = true
            };

            // Mã hóa mật khẩu
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            // Giữ Password nếu project của bạn vẫn đang dùng cột này
            user.Password = string.Empty;

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Tạo tài khoản thành công.",
                id = user.UserId
            });
        }

        // =====================================================
        // PUT: api/users/{id}/reset-password
        // Đặt lại mật khẩu
        // =====================================================
        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int id,
            [FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { message = "Mật khẩu mới không được để trống." });

            if (request.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự." });

            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            user.PasswordHash = _hasher.HashPassword(user, request.NewPassword);

            // Giữ Password để tương thích với cấu trúc hiện tại
            user.Password = string.Empty;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Đặt lại mật khẩu thành công."
            });
        }

        // =====================================================
        // PUT: api/users/{id}/toggle-lock
        // Khóa / mở khóa tài khoản
        // =====================================================
        [HttpPut("{id}/toggle-lock")]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            user.IsActive = !user.IsActive;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = user.IsActive
                    ? "Đã mở khóa tài khoản."
                    : "Đã khóa tài khoản.",
                isActive = user.IsActive
            });
        }
    }

    // =====================================================
    // DTO: Hiển thị User
    // =====================================================
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    // =====================================================
    // DTO: Tạo User
    // =====================================================
    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = "Cashier";
    }

    // =====================================================
    // DTO: Đặt lại mật khẩu
    // =====================================================
    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
