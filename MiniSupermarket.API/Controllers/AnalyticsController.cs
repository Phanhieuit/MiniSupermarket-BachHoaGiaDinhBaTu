using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;

namespace MiniSupermarket.API.Controllers;

// TC44,45,46,48: dữ liệu dashboard và CSV; chỉ Admin được xem báo cáo toàn hệ thống.
[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public sealed class AnalyticsController(SupermarketDbContext db, IConfiguration configuration) : ControllerBase
{
    private static DateTime LocalToday => DateTime.UtcNow.AddHours(7).Date;

    private static (DateTime Start, DateTime End) UtcDayRange(DateTime localDay)
    {
        var start = localDay.Date.AddHours(-7);
        return (start, start.AddDays(1));
    }

    // TC44: 4 thẻ KPI: doanh thu hôm nay, tổng hóa đơn, KH mới, tồn kho <= 10.
    [HttpGet("dashboard-summary")]
    public async Task<IActionResult> DashboardSummary()
    {
        var (start, end) = UtcDayRange(LocalToday);
        var todayRevenue = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= start && o.CreatedAt < end)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
        var totalInvoices = await db.Orders.AsNoTracking().CountAsync();
        var newCustomersToday = await db.Customers.AsNoTracking()
            .CountAsync(c => c.RegisteredAtUtc >= start && c.RegisteredAtUtc < end);
        var lowStockProducts = await db.Products.AsNoTracking()
            .CountAsync(p => p.StockQuantity <= 10);

        return Ok(new
        {
            todayRevenue,
            totalInvoices,
            newCustomersToday,
            lowStockProducts,
            localDate = LocalToday.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        });
    }

    // TC45: API nhóm doanh thu bằng SQL Stored Procedure (theo UTC+7).
    // day: 7 ngày, week: 8 tuần (Thứ Hai), month: 12 tháng.
    [HttpGet("revenue-trend")]
    public async Task<IActionResult> RevenueTrend(
        [FromQuery] string period = "day", [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        period = (period ?? "day").Trim().ToLowerInvariant();
        if (period != "day" && period != "week" && period != "month")
            return BadRequest(new { message = "period phải là day, week hoặc month." });
        var toDate = (to ?? LocalToday).Date;
        var fromDate = (from ?? (period switch
        {
            "week" => toDate.AddDays(-7 * 7),
            "month" => toDate.AddMonths(-11),
            _ => toDate.AddDays(-6)
        })).Date;
        if (fromDate.Year < 2000 || toDate.Year > 2100 || fromDate > toDate)
            return BadRequest(new { message = "Khoảng ngày không hợp lệ." });

        DateTime startLocal = period switch
        {
            "week" => fromDate.AddDays(-(((int)fromDate.DayOfWeek + 6) % 7)),
            "month" => new DateTime(fromDate.Year, fromDate.Month, 1),
            _ => fromDate
        };
        DateTime lastBucket = period switch
        {
            "week" => toDate.AddDays(-(((int)toDate.DayOfWeek + 6) % 7)),
            "month" => new DateTime(toDate.Year, toDate.Month, 1),
            _ => toDate
        };

        var buckets = new List<DateTime>();
        for (DateTime cursor = startLocal; cursor <= lastBucket;
             cursor = period switch
             {
                 "day" => cursor.AddDays(1),
                 "week" => cursor.AddDays(7),
                 _ => cursor.AddMonths(1)
             })
        {
            buckets.Add(cursor);
            if (buckets.Count > (period == "day" ? 92 : period == "week" ? 53 : 24))
                return BadRequest(new { message = "Khoảng thống kê quá dài, hãy thu hẹp thời gian." });
        }
        DateTime endLocal = period switch
        {
            "day" => lastBucket.AddDays(1),
            "week" => lastBucket.AddDays(7),
            _ => lastBucket.AddMonths(1)
        };

        var rows = new Dictionary<DateTime, TrendRow>();
        await using var connection = new SqlConnection(
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Thiếu DefaultConnection."));
        await connection.OpenAsync();
        await using (var command = new SqlCommand("dbo.usp_RevenueTrend", connection)
        {
            CommandType = CommandType.StoredProcedure
        })
        {
            command.Parameters.Add("@StartUtc", SqlDbType.DateTime2).Value = startLocal.AddHours(-7);
            command.Parameters.Add("@EndUtc", SqlDbType.DateTime2).Value = endLocal.AddHours(-7);
            command.Parameters.Add("@Grouping", SqlDbType.NVarChar, 10).Value = period;
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var key = reader.GetDateTime(0).Date;
                rows[key] = new TrendRow(key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    reader.GetInt32(1), reader.GetDecimal(2));
            }
        }

        var series = buckets.Select(key => rows.TryGetValue(key, out var found)
            ? found : new TrendRow(key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), 0, 0))
            .ToList();
        return Ok(new { period, fromDate, toDate, data = series });
    }

    public record TrendRow(string Date, int TotalOrders, decimal TotalRevenue);

    // TC46: GROUP BY OrderDetails.ProductId, tổng số lượng bán ra.
    [HttpGet("top-products")]
    public async Task<IActionResult> TopProducts([FromQuery] int take = 5)
    {
        if (take is < 1 or > 20)
            return BadRequest(new { message = "take từ 1 đến 20." });

        var rows = await db.OrderDetails.AsNoTracking()
            .GroupBy(d => d.ProductId)
            .Select(g => new
            {
                productId = g.Key,
                productName = g.Max(d => d.ProductName),
                quantitySold = g.Sum(d => d.Quantity),
                revenue = g.Sum(d => d.Quantity * d.UnitPrice)
            })
            .OrderByDescending(g => g.quantitySold)
            .ThenBy(g => g.productId)
            .Take(take)
            .ToListAsync();
        return Ok(rows);
    }

    // TC48: xuất CSV Unicode (Excel mở trực tiếp), chống công thức CSV injection.
    [HttpGet("export/products.csv")]
    public async Task<IActionResult> ExportProducts()
    {
        var products = await db.Products.AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.ProductId)
            .Select(p => new
            {
                p.ProductId,
                p.Barcode,
                p.ProductName,
                p.Price,
                p.StockQuantity,
                categoryName = p.Category != null ? p.Category.CategoryName : ""
            }).ToListAsync();

        var csv = new StringBuilder("ProductId,Barcode,ProductName,Price,StockQuantity,CategoryName\r\n");
        foreach (var p in products)
            CsvLine(csv, p.ProductId.ToString(), p.Barcode, p.ProductName,
                p.Price.ToString(CultureInfo.InvariantCulture), p.StockQuantity.ToString(), p.categoryName);

        return CsvFile(csv, "DanhMucSanPham.csv");
    }

    [HttpGet("export/orders.csv")]
    public async Task<IActionResult> ExportOrders()
    {
        var orders = await db.Orders.AsNoTracking()
            .OrderBy(o => o.OrderId)
            .Select(o => new
            {
                o.OrderId,
                o.CreatedAt,
                o.CashierUsername,
                o.CustomerId,
                o.TotalAmount,
                o.DiscountAmount,
                o.CashReceived
            }).ToListAsync();

        var csv = new StringBuilder("OrderId,CreatedAtVietnam,CashierUsername,CustomerId,TotalAmount,DiscountAmount,CashReceived\r\n");
        foreach (var o in orders)
            CsvLine(csv, o.OrderId.ToString(),
                o.CreatedAt.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                o.CashierUsername, o.CustomerId?.ToString() ?? "",
                o.TotalAmount.ToString(CultureInfo.InvariantCulture),
                o.DiscountAmount.ToString(CultureInfo.InvariantCulture),
                o.CashReceived.ToString(CultureInfo.InvariantCulture));
        return CsvFile(csv, "BaoCaoHoaDon.csv");
    }

    private FileContentResult CsvFile(StringBuilder csv, string name) =>
        File(new UTF8Encoding(true).GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(),
            "text/csv; charset=utf-8", name);

    private static void CsvLine(StringBuilder sb, params string[] values)
    {
        sb.AppendJoin(',', values.Select(CsvCell));
        sb.Append("\r\n");
    }

    private static string CsvCell(string? value)
    {
        value ??= "";
        // Excel có thể thực thi các ô bắt đầu bằng công thức; đặt nháy đơn phía trước.
        var trimLeft = value.TrimStart(' ', '\t', '\r', '\n');
        if (trimLeft.Length > 0 && "=+-@".Contains(trimLeft[0]))
            value = "'" + value;
        return '"' + value.Replace("\"", "\"\"") + '"';
    }
}
