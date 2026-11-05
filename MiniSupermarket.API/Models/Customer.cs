
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Tên khách hàng không được để trống")]
    [StringLength(100, ErrorMessage = "Tên khách hàng tối đa 100 ký tự")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [RegularExpression(
        @"^0\d{9}$",
        ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
    public string? Address { get; set; }

    [Required]
    [StringLength(30)]
    public string MembershipRank { get; set; } = "Chuẩn";

    [Range(0, int.MaxValue,
        ErrorMessage = "Điểm thưởng không được âm")]
    public int RewardPoints { get; set; }

    // TC44: chỉ khách đăng ký sau migration mới có mốc thời gian này.
    public DateTime? RegisteredAtUtc { get; set; }
}
