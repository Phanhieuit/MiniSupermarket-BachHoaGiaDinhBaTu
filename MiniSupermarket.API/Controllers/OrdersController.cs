using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using MiniSupermarket.API.DTOs;
using Microsoft.Data.SqlClient;

namespace MiniSupermarket.API.Controllers;

[ApiController, Route("api/orders"), Authorize(Roles = "Admin,Cashier")]
public class OrdersController(SupermarketDbContext db) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var query = db.Orders.AsNoTracking();

        if (!User.IsInRole("Admin"))
        {
            query = query.Where(o =>
                o.CashierUsername == User.Identity!.Name);
        }

        var orders = await query
            .OrderByDescending(o => o.OrderId)
            .Take(100)
            .Select(o => new OrderSummaryResponse
            {
                OrderId = o.OrderId,
                CreatedAt = o.CreatedAt,
                CashierUsername = o.CashierUsername,
                CustomerId = o.CustomerId,
                TotalAmount = o.TotalAmount,
                DiscountAmount = o.DiscountAmount,
                CashReceived = o.CashReceived,
                Change = o.CashReceived - o.TotalAmount
            })
            .ToListAsync();

        return Ok(orders);
    }



    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var query = db.Orders.AsNoTracking()
            .Where(o => o.OrderId == id);

        if (!User.IsInRole("Admin"))
        {
            query = query.Where(o =>
                o.CashierUsername == User.Identity!.Name);
        }

        var order = await query
            .Select(o => new OrderDetailResponse
            {
                OrderId = o.OrderId,
                CreatedAt = o.CreatedAt,
                CashierUsername = o.CashierUsername,
                CustomerId = o.CustomerId,

                CustomerName = o.Customer == null
                    ? "Khách vãng lai"
                    : o.Customer.CustomerName,

                TotalAmount = o.TotalAmount,
                DiscountAmount = o.DiscountAmount,
                CashReceived = o.CashReceived,
                Change = o.CashReceived - o.TotalAmount,

                OrderDetails = o.OrderDetails
                    .Select(d => new OrderItemResponse
                    {
                        OrderDetailId = d.OrderDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.Quantity * d.UnitPrice
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (order == null)
            return NotFound();

        return Ok(order);
    }


    // TC35: Thanh toán POS bằng SqlTransaction: hóa đơn, chi tiết, tồn kho,
    // và điểm khách hàng chỉ được lưu khi toàn bộ giao dịch thành công.
    // Giữ nguyên api/orders/checkout, CheckoutRequest và CheckoutResponse của WinForms.
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(
        [FromBody] CheckoutRequest request,
        [FromServices] IConfiguration configuration,
        [FromServices] ILogger<OrdersController> logger)
    {
        if (request?.Items == null || request.Items.Count == 0 ||
            request.Items.Count > 200 ||
            request.Items.Any(i => i.ProductId <= 0 || i.Quantity <= 0 ||
                i.Quantity > 10000 || i.UnitPrice <= 0) ||
            request.Items.Select(i => i.ProductId).Distinct().Count() != request.Items.Count)
        {
            return BadRequest(new { message = "Giỏ hàng không hợp lệ." });
        }

        var cashier = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(cashier))
            return Unauthorized(new { message = "Chưa đăng nhập." });

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Thiếu chuỗi kết nối DefaultConnection.");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);
        var committed = false;

        try
        {
            var orderItems = new List<CheckoutOrderItem>();
            decimal total = 0;

            // Đọc giá/tồn kho dưới UPDLOCK + HOLDLOCK; các sản phẩm được khóa
            // theo thứ tự ProductId nhằm giảm khả năng deadlock giữa hai quầy.
            foreach (var item in request.Items.OrderBy(i => i.ProductId))
            {
                using var productCommand = new SqlCommand(@"
                    SELECT ProductName, Price, StockQuantity
                    FROM dbo.Products WITH (UPDLOCK, HOLDLOCK)
                    WHERE ProductId = @ProductId", connection, transaction);
                productCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;

                string productName = string.Empty;
                decimal currentPrice = 0;
                int stock = 0;
                bool found;
                using (var reader = await productCommand.ExecuteReaderAsync(CommandBehavior.SingleRow))
                {
                    found = await reader.ReadAsync();
                    if (found)
                    {
                        productName = reader.GetString(0);
                        currentPrice = reader.GetDecimal(1);
                        stock = reader.GetInt32(2);
                    }
                }

                // Reader đã được đóng trước khi Rollback.
                if (!found || stock < item.Quantity)
                {
                    transaction.Rollback();
                    return BadRequest(new { message = "Sản phẩm không tồn tại hoặc không đủ tồn kho." });
                }
                if (currentPrice != item.UnitPrice)
                {
                    transaction.Rollback();
                    return BadRequest(new { message = "Giá đã thay đổi, vui lòng hủy giỏ và quét lại." });
                }
                orderItems.Add(new CheckoutOrderItem(item.ProductId, productName, item.Quantity, currentPrice));
                total += item.Quantity * currentPrice;
            }

            if (total <= 0 || total > 1000000000m)
            {
                transaction.Rollback();
                return BadRequest(new { message = "Tổng tiền không hợp lệ." });
            }

            int? customerId = null;
            int currentPoints = 0;
            if (!string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                using var customerCommand = new SqlCommand(@"
                    SELECT CustomerId, RewardPoints
                    FROM dbo.Customers WITH (UPDLOCK, HOLDLOCK)
                    WHERE PhoneNumber = @Phone", connection, transaction);
                customerCommand.Parameters.Add("@Phone", SqlDbType.NVarChar, 50).Value =
                    request.CustomerPhone.Trim();
                using var reader = await customerCommand.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (!await reader.ReadAsync())
                {
                    await reader.CloseAsync();
                    transaction.Rollback();
                    return BadRequest(new { message = "Không tìm thấy khách hàng thành viên." });
                }
                customerId = reader.GetInt32(0);
                currentPoints = reader.GetInt32(1);
            }

            // TC47: duy nhất 50 điểm = 50.000đ, chỉ khi hóa đơn > 50.000đ.
            const int redeemPoints = 50;
            const decimal redeemValue = 50000m;
            if (request.UseRewardPoints &&
                (!customerId.HasValue || currentPoints < redeemPoints || total <= redeemValue))
            {
                transaction.Rollback();
                return BadRequest(new { message = "Cần khách thành viên có ít nhất 50 điểm và đơn hàng trên 50.000đ để đổi điểm." });
            }
            decimal discount = request.UseRewardPoints ? redeemValue : 0m;
            decimal finalAmount = total - discount;
            int pointsSpent = request.UseRewardPoints ? redeemPoints : 0;
            int pointsEarned = customerId.HasValue ? (int)(finalAmount / 10000m) : 0;
            if (request.CashReceived < finalAmount || request.CashReceived > 1000000000m)
            {
                transaction.Rollback();
                return BadRequest(new { message = "Tiền khách đưa chưa đủ hoặc không hợp lệ." });
            }

            // Lưu hóa đơn - mọi SqlCommand dùng cùng connection & transaction.
            int orderId;
            using (var createOrder = new SqlCommand(@"
                INSERT INTO dbo.Orders
                    (CreatedAt, CashierUsername, CustomerId, TotalAmount, CashReceived, DiscountAmount)
                OUTPUT INSERTED.OrderId
                VALUES (@CreatedAt, @Cashier, @CustomerId, @Total, @Cash, @Discount)",
                connection, transaction))
            {
                createOrder.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value = DateTime.UtcNow;
                createOrder.Parameters.Add("@Cashier", SqlDbType.NVarChar, 50).Value = cashier;
                createOrder.Parameters.Add("@CustomerId", SqlDbType.Int).Value =
                    customerId.HasValue ? (object)customerId.Value : DBNull.Value;
                AddMoney(createOrder, "@Total", finalAmount);
                AddMoney(createOrder, "@Cash", request.CashReceived);
                AddMoney(createOrder, "@Discount", discount);
                orderId = Convert.ToInt32(await createOrder.ExecuteScalarAsync());
            }

            foreach (var item in orderItems)
            {
                using (var createDetail = new SqlCommand(@"
                    INSERT INTO dbo.OrderDetails
                        (OrderId, ProductId, ProductName, Quantity, UnitPrice)
                    VALUES (@OrderId, @ProductId, @ProductName, @Quantity, @Price)",
                    connection, transaction))
                {
                    createDetail.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    createDetail.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                    createDetail.Parameters.Add("@ProductName", SqlDbType.NVarChar, 200).Value = item.ProductName;
                    createDetail.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                    AddMoney(createDetail, "@Price", item.UnitPrice);
                    await createDetail.ExecuteNonQueryAsync();
                }

                using var updateStock = new SqlCommand(@"
                    UPDATE dbo.Products
                    SET StockQuantity = StockQuantity - @Quantity
                    WHERE ProductId = @ProductId AND StockQuantity >= @Quantity",
                    connection, transaction);
                updateStock.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                updateStock.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;

                if (await updateStock.ExecuteNonQueryAsync() != 1)
                {
                    transaction.Rollback(); // TC35: không lưu hóa đơn rác
                    return Conflict(new { message = "Không đủ tồn kho. Đã hoàn tác giao dịch." });
                }
            }

            // TC47: Trừ 50 điểm và cộng điểm tích lũy trong CÙNG SqlTransaction.
            if (customerId.HasValue && (pointsSpent > 0 || pointsEarned > 0))
            {

                using var updatePoints = new SqlCommand(@"
    UPDATE dbo.Customers
    SET RewardPoints = RewardPoints - @Spent + @Earned
    WHERE CustomerId = @CustomerId
      AND RewardPoints >= @Spent
      AND CAST(RewardPoints AS BIGINT)
          - CAST(@Spent AS BIGINT)
          + CAST(@Earned AS BIGINT) <= 2147483647",
                    connection, transaction);

                updatePoints.Parameters.Add(
                    "@Spent", SqlDbType.Int).Value = pointsSpent;

                updatePoints.Parameters.Add(
                    "@Earned", SqlDbType.Int).Value = pointsEarned;

                updatePoints.Parameters.Add(
                    "@CustomerId", SqlDbType.Int).Value = customerId.Value;

                if (await updatePoints.ExecuteNonQueryAsync() != 1)
                {
                    transaction.Rollback();

                    return Conflict(new
                    {
                        message = "Không thể cập nhật điểm; toàn bộ giao dịch đã hoàn tác."
                    });
                }

            }

            transaction.Commit();
            committed = true;
            return Ok(new CheckoutResponse
            {
                OrderId = orderId,
                TotalAmount = finalAmount,
                Change = request.CashReceived - finalAmount,
                DiscountAmount = discount,
                PointsSpent = pointsSpent,
                PointsEarned = pointsEarned
            });
        }
        catch (Exception ex)
        {
            if (!committed)
            {
                try { transaction.Rollback(); }
                catch (InvalidOperationException) { /* transaction đã hoàn tất */ }
                catch (SqlException rollbackError)
                {
                    logger.LogError(rollbackError, "Lỗi khi rollback giao dịch POS.");
                }
            }
            logger.LogError(ex, "Giao dịch POS thất bại.");
            return StatusCode(500, new { message =
                "Lỗi thanh toán, giao dịch không được xác nhận. Kiểm tra hóa đơn trước khi thử lại." });
        }
    }

    private static void AddMoney(SqlCommand command, string name, decimal value)
    {
        var parameter = command.Parameters.Add(name, SqlDbType.Decimal);
        parameter.Precision = 18;
        parameter.Scale = 2;
        parameter.Value = value;
    }

    private sealed record CheckoutOrderItem(
        int ProductId, string ProductName, int Quantity, decimal UnitPrice);

}


[ApiController, Route("api/reports"), Authorize(Roles = "Admin")]
public class ReportsController(SupermarketDbContext db) : ControllerBase
{
    [HttpGet("daily")]
    public async Task<IActionResult> Daily(DateTime date)
    {
        // Ngày bán hàng theo giờ Việt Nam (UTC+7), dữ liệu lưu theo UTC.
        var start = DateTime.SpecifyKind(date.Date.AddHours(-7), DateTimeKind.Utc);
        var end = start.AddDays(1);
        var orders = await db.Orders.AsNoTracking().Include(o => o.OrderDetails)
            .Where(o => o.CreatedAt >= start && o.CreatedAt < end).ToListAsync();
        var best = orders.SelectMany(o => o.OrderDetails).GroupBy(i => new { i.ProductId, i.ProductName })
            .OrderByDescending(g => g.Sum(i => i.Quantity)).FirstOrDefault();
        return Ok(new { totalOrders = orders.Count, totalRevenue = orders.Sum(o => o.TotalAmount),
            bestSeller = best?.Key.ProductName ?? "Chưa có dữ liệu" });
    }
}

