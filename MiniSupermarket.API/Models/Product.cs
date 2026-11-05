
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Mã vạch không được để trống")]
    [StringLength(50, ErrorMessage = "Mã vạch tối đa 50 ký tự")]
    public string Barcode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự")]
    public string ProductName { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000000",
        ErrorMessage = "Giá bán phải lớn hơn 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessage = "Số lượng tồn kho không được âm")]
    public int StockQuantity { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Danh mục không hợp lệ")]
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
   
}
