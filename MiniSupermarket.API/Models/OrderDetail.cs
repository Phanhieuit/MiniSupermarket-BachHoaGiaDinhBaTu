
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models;

public class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    [Range(1, int.MaxValue,
        ErrorMessage = "Mã sản phẩm không hợp lệ")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue,
        ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "1000000000",
        ErrorMessage = "Đơn giá phải lớn hơn 0")]
    public decimal UnitPrice { get; set; }
}
