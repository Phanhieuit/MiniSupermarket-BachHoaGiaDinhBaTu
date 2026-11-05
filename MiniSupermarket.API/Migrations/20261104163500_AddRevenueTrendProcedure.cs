using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MiniSupermarket.API.Data;

#nullable disable
namespace MiniSupermarket.API.Migrations;

// TC45: thống kê bằng GROUP BY trên SQL Server thay vì duyệt tất cả đơn trên client.
[DbContext(typeof(SupermarketDbContext))]
[Migration("20261104163500_AddRevenueTrendProcedure")]
public sealed class AddRevenueTrendProcedure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE dbo.usp_RevenueTrend
    @StartUtc DATETIME2,
    @EndUtc DATETIME2,
    @Grouping NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    IF @StartUtc >= @EndUtc OR @Grouping NOT IN (N'day', N'week', N'month')
        THROW 51002, N'Tham số thống kê không hợp lệ.', 1;

    ;WITH LocalOrders AS (
        SELECT CAST(DATEADD(HOUR, 7, CreatedAt) AS DATE) AS LocalDate, TotalAmount
        FROM dbo.Orders
        WHERE CreatedAt >= @StartUtc AND CreatedAt < @EndUtc
    ), Bucketed AS (
        SELECT CASE
            WHEN @Grouping = N'day' THEN LocalDate
            WHEN @Grouping = N'week' THEN
                DATEADD(DAY, -DATEDIFF(DAY, '19000101', LocalDate) % 7, LocalDate)
            ELSE DATEFROMPARTS(YEAR(LocalDate), MONTH(LocalDate), 1)
        END AS BucketDate,
        TotalAmount FROM LocalOrders
    )
    SELECT BucketDate,
           COUNT(*) AS TotalOrders,
           CONVERT(DECIMAL(18,2), ISNULL(SUM(TotalAmount), 0)) AS TotalRevenue
    FROM Bucketed
    GROUP BY BucketDate
    ORDER BY BucketDate;
END");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_RevenueTrend");
    }
}
