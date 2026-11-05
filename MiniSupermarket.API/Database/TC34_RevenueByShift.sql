USE BachHoaGiaDinhBaTu_MigrationTestDb;
GO

CREATE OR ALTER PROCEDURE dbo.usp_RevenueByShift
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
END
GO

-- Ví dụ: ca đêm 27/10, gồm 22:00-06:00 sáng ngày 28/10 giờ VN.
EXEC dbo.usp_RevenueByShift @ReportDate = '2026-10-27', @Shift = 3;
GO
