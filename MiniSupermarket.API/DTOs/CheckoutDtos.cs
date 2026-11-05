
using System.Collections.Generic;

namespace MiniSupermarket.API.DTOs;

public class CheckoutRequest
{
    public string? CustomerPhone { get; set; }

    public decimal CashReceived { get; set; }

    // TC47: chỉ đổi 50 điểm lấy 50.000đ khi đã xác thực khách.
    public bool UseRewardPoints { get; set; }

    public List<CheckoutItem> Items { get; set; } = new();
}

public record CheckoutItem(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);

public class CheckoutResponse
{
    public int OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal Change { get; set; }

    public decimal DiscountAmount { get; set; }
    public int PointsSpent { get; set; }
    public int PointsEarned { get; set; }
}
