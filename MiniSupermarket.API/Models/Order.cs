
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required(ErrorMessage = "Nhân viên thu ngân không được để trống")]
    [StringLength(50)]
    public string CashierUsername { get; set; } = string.Empty;

    public int? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [Range(typeof(decimal), "0.01", "1000000000",
        ErrorMessage = "Tổng tiền phải lớn hơn 0")]
    public decimal TotalAmount { get; set; }

    // TC47: số tiền giảm bằng điểm thành viên (0 hoặc 50.000).
    public decimal DiscountAmount { get; set; }

    [Range(typeof(decimal), "0", "1000000000",
        ErrorMessage = "Tiền khách đưa không được âm")]
    public decimal CashReceived { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = new();
}
