using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using MiniSupermarket.API.DTOs;
namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SupermarketDbContext _db;
        private readonly ILogger<AuthController> _logger;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(
            IConfiguration configuration,
            SupermarketDbContext db,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
        }

        // Endpoint Đăng nhập: POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            string username = (request.Username ?? string.Empty).Trim();

            // 1. Đăng nhập bằng tài khoản trong database
            try
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

                if (user != null)
                {
                    if (!user.IsActive)
                        return Unauthorized(new { success = false, message = "Tài khoản đã bị khóa!" });

                    var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

                    if (result != PasswordVerificationResult.Failed)
                    {
                        var token = GenerateJwtToken(user.Username, user.Role);
                        return Ok(new LoginResponseDto
                        {
                            Success = true,
                            Token = token,
                            Role = user.Role
                        });
                    }

                    return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi truy vấn cơ sở dữ liệu
                _logger.LogWarning(ex, "Không truy vấn được bảng Users.");
            }
            return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu!" });
        }

        private string GenerateJwtToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Lấy khóa bí mật từ appsettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "BachHoaGiaDinhBaTuRestfulWebApi2026SecretKeyChangeBeforeProduction!!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Thời hạn token là 2 tiếng
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    
}