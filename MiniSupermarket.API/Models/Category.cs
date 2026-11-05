
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [StringLength(200, ErrorMessage = "Tên danh mục tối đa 200 ký tự")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}
