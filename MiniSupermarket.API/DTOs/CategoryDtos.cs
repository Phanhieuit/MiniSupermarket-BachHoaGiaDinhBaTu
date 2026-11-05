using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.DTOs;

public class CategoryRequest
{
    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [StringLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public record CategoryResponse(
    int CategoryId,
    string CategoryName,
    string? Description
);