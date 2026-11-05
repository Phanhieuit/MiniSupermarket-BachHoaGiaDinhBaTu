using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MiniSupermarket.API.Data;

#nullable disable

namespace MiniSupermarket.API.Migrations;

// TC34: triển khai Stored Procedure bằng migration, không thay đổi dữ liệu hiện có.
[DbContext(typeof(SupermarketDbContext))]
[Migration("20261029190000_AddRevenueByShiftProcedure")]
public sealed class AddRevenueByShiftProcedure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE dbo.usp_RevenueByShift
    @ReportDate DATE,
    @Shift TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    IF @ReportDate IS NULL OR @Shift NOT BETWEEN 1 AND 3 OR @Shift IS NULL
    BEGIN
        THROW 51001, N'Ngày hoặc ca không hợp lệ (ca 1, 2, 3).', 1;
    END;

    -- Ngày/giờ theo Việt Nam UTC+7; Orders.CreatedAt lưu theo UTC.
    -- Ca 1: 06:00-14:00; ca 2: 14:00-22:00; ca 3: 22:00-06:00 hôm sau.
    DECLARE @StartVN DATETIME2 = DATEADD(HOUR,
        CASE WHEN @Shift = 1 THEN 6 WHEN @Shift = 2 THEN 14 ELSE 22 END,
        CAST(@ReportDate AS DATETIME2));
    DECLARE @EndVN DATETIME2 = DATEADD(HOUR,
        CASE WHEN @Shift = 1 THEN 14 WHEN @Shift = 2 THEN 22 ELSE 30 END,
        CAST(@ReportDate AS DATETIME2));
    DECLARE @StartUTC DATETIME2 = DATEADD(HOUR, -7, @StartVN);
    DECLARE @EndUTC DATETIME2 = DATEADD(HOUR, -7, @EndVN);

    SELECT COUNT(*) AS TotalOrders,
           ISNULL(SUM(TotalAmount), CONVERT(DECIMAL(18,2), 0)) AS TotalRevenue
    FROM dbo.Orders
    WHERE CreatedAt >= @StartUTC AND CreatedAt < @EndUTC;
END");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_RevenueByShift");
    }
}
