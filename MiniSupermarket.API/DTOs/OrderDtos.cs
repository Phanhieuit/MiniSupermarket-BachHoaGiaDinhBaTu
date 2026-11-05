
namespace MiniSupermarket.API.DTOs;

// DTO danh sách hóa đơn
public class OrderSummaryResponse
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CashierUsername { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal CashReceived { get; set; }
    public decimal Change { get; set; }
}

// DTO từng sản phẩm trong hóa đơn
public class OrderItemResponse
{
    public int OrderDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

// DTO chi tiết hóa đơn
public class OrderDetailResponse : OrderSummaryResponse
{
    public string CustomerName { get; set; } = string.Empty;

    public List<OrderItemResponse> OrderDetails { get; set; } = new();
}
