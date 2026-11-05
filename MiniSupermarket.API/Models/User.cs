
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vai trò không được để trống")]
    [StringLength(20)]
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    // Giữ để tương thích với cơ chế dữ liệu mẫu hiện tại.
    // Không lưu mật khẩu dạng văn bản thuần khi triển khai thực tế.
    public string Password { get; set; } = string.Empty;
}
