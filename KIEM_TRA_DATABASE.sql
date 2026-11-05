-- Script chỉ KIỂM TRA dữ liệu: không xóa, không ghi đè dữ liệu.
-- Chọn/kiểm tra database BachHoaGiaDinhBaTuDb trong SSMS.
USE [BachHoaGiaDinhBaTuDb];
GO
SELECT CategoryId, CategoryName, Description FROM dbo.Categories ORDER BY CategoryId;
SELECT ProductId, Barcode, ProductName, Price, StockQuantity, CategoryId FROM dbo.Products ORDER BY ProductId;
SELECT (SELECT COUNT(*) FROM dbo.Categories) AS SoDanhMuc,
       (SELECT COUNT(*) FROM dbo.Products) AS SoSanPham,
       (SELECT COUNT(*) FROM dbo.Customers) AS SoKhachHang;
GO
-- Tình trạng đơn hàng sau khi chạy API (script Orders.sql sẽ tạo bảng khi cần):
IF OBJECT_ID(N'dbo.Orders', N'U') IS NOT NULL
    SELECT TOP (20) * FROM dbo.Orders ORDER BY OrderId DESC;
IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NOT NULL
    SELECT TOP (20) * FROM dbo.OrderDetails ORDER BY OrderDetailId DESC;
GO
