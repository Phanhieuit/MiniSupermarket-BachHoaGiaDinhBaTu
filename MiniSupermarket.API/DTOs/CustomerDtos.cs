
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.DTOs;

public class CustomerRequest
{
    [Required(ErrorMessage = "Tên khách hàng không được để trống")]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [RegularExpression(@"^0\d{9}$",
        ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Address { get; set; }
}

public class CustomerResponse
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string MembershipRank { get; set; } = string.Empty;
    public int RewardPoints { get; set; }
}
