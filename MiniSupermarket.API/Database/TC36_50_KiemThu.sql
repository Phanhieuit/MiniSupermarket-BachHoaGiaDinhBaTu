-- TC36-50: script chỉ ĐỌC, không thay đổi bất kỳ hàng nào.
USE BachHoaGiaDinhBaTu_MigrationTestDb;
GO
-- TC43: mặt hàng dưới ngưỡng
SELECT ProductId, Barcode, ProductName, StockQuantity
FROM dbo.Products WHERE StockQuantity <= 10 ORDER BY StockQuantity, ProductId;

-- TC44: tổng hóa đơn, tổng tồn nguy cơ hết hàng
SELECT COUNT(*) AS TotalInvoices FROM dbo.Orders;
SELECT COUNT(*) AS LowStockProducts FROM dbo.Products WHERE StockQuantity <= 10;
SELECT COUNT(*) AS RegisteredToday
FROM dbo.Customers
WHERE RegisteredAtUtc >= DATEADD(HOUR, -7, CONVERT(DATE, DATEADD(HOUR, 7, SYSUTCDATETIME())))
  AND RegisteredAtUtc < DATEADD(DAY, 1, DATEADD(HOUR, -7, CONVERT(DATE, DATEADD(HOUR, 7, SYSUTCDATETIME()))));

-- TC46: Top 5 bán chạy
SELECT TOP (5) ProductId, SUM(Quantity) AS QuantitySold,
       SUM(Quantity * UnitPrice) AS Revenue
FROM dbo.OrderDetails
GROUP BY ProductId
ORDER BY QuantitySold DESC, ProductId ASC;

-- TC47: đối chiếu tiền sau giảm điểm, không thay đổi dữ liệu
SELECT TOP (10) OrderId, CreatedAt, TotalAmount, DiscountAmount, CashReceived
FROM dbo.Orders ORDER BY OrderId DESC;
SELECT TOP (10) CustomerId, CustomerName, RewardPoints, RegisteredAtUtc
FROM dbo.Customers ORDER BY CustomerId DESC;
