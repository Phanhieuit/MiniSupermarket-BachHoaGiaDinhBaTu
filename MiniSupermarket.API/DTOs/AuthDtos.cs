
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
