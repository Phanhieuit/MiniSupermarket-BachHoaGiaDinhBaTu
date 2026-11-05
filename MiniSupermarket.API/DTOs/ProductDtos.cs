using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.DTOs;

public class ProductRequest
{
    [Required, StringLength(50)]
    public string Barcode { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Range(0.01, 1_000_000_000)]
    public decimal Price { get; set; }

    [Range(0, 1_000_000)]
    public int StockQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}

public class ProductResponse
{
    public int ProductId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}