-- Chạy trên database BachHoaGiaDinhBaTuDb (chọn đúng database trong SSMS trước khi chạy).
-- Script được API chạy tự động khi khởi động và có thể chạy lại.
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.Orders', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.SaleOrders', N'U') IS NOT NULL
        THROW 50001, N'Tồn tại cả Orders và SaleOrders. Cần đối chiếu dữ liệu trước khi chuyển đổi.', 1;
    IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.SaleOrderItems', N'U') IS NOT NULL
        THROW 50002, N'Tồn tại cả OrderDetails và SaleOrderItems. Cần đối chiếu dữ liệu trước khi chuyển đổi.', 1;

    IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL AND OBJECT_ID(N'dbo.SaleOrders', N'U') IS NOT NULL
    BEGIN
        EXEC sp_rename N'dbo.SaleOrders', N'Orders';
        EXEC sp_rename N'dbo.Orders.SaleOrderId', N'OrderId', N'COLUMN';
    END;
    IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Orders (
            OrderId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
            CreatedAt datetime2 NOT NULL,
            CashierUsername nvarchar(max) NOT NULL,
            CustomerId int NULL,
            TotalAmount decimal(18,2) NOT NULL,
            CashReceived decimal(18,2) NOT NULL);
    END;
    IF COL_LENGTH(N'dbo.Orders', N'CustomerId') IS NULL
        ALTER TABLE dbo.Orders ADD CustomerId int NULL;
    IF COL_LENGTH(N'dbo.Orders', N'CashReceived') IS NULL
    BEGIN
        ALTER TABLE dbo.Orders ADD CashReceived decimal(18,2) NOT NULL
            CONSTRAINT DF_Orders_CashReceived DEFAULT (0);
        -- Hóa đơn cũ không lưu tiền khách đưa: khởi tạo bằng tổng tiền.
        EXEC(N'UPDATE dbo.Orders SET CashReceived = TotalAmount');
    END;

    IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NULL AND OBJECT_ID(N'dbo.SaleOrderItems', N'U') IS NOT NULL
    BEGIN
        EXEC sp_rename N'dbo.SaleOrderItems', N'OrderDetails';
        EXEC sp_rename N'dbo.OrderDetails.SaleOrderItemId', N'OrderDetailId', N'COLUMN';
        EXEC sp_rename N'dbo.OrderDetails.SaleOrderId', N'OrderId', N'COLUMN';
    END;
    IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.OrderDetails (
            OrderDetailId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderDetails PRIMARY KEY,
            OrderId int NOT NULL,
            ProductId int NOT NULL,
            ProductName nvarchar(max) NOT NULL,
            Quantity int NOT NULL,
            UnitPrice decimal(18,2) NOT NULL,
            CONSTRAINT FK_OrderDetails_Orders_OrderId FOREIGN KEY (OrderId)
                REFERENCES dbo.Orders(OrderId) ON DELETE CASCADE);
    END;
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'dbo.Orders') AND referenced_object_id = OBJECT_ID(N'dbo.Customers'))
        EXEC(N'ALTER TABLE dbo.Orders ADD CONSTRAINT FK_Orders_Customers_CustomerId FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId)');
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Orders') AND name = N'IX_Orders_CustomerId')
        EXEC(N'CREATE INDEX IX_Orders_CustomerId ON dbo.Orders(CustomerId)');
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.OrderDetails') AND name = N'IX_OrderDetails_OrderId')
        EXEC(N'CREATE INDEX IX_OrderDetails_OrderId ON dbo.OrderDetails(OrderId)');
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
