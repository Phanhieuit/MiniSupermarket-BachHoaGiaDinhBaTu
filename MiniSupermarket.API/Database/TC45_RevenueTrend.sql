-- Script TC45 tham khảo. Migrations sẽ tạo usp_RevenueTrend khi chạy Update-Database.
-- Chạy thủ công CHỈ nếu bạn không dùng migration và đã tạo procedure từ
-- 20261104163500_AddRevenueTrendProcedure.cs.
USE BachHoaGiaDinhBaTu_MigrationTestDb;
GO
EXEC dbo.usp_RevenueTrend
    @StartUtc = '2026-10-31T17:00:00',
    @EndUtc = '2026-11-07T17:00:00',
    @Grouping = N'day';
GO
