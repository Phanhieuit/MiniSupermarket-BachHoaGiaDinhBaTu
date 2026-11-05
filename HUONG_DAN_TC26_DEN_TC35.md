# Bách hóa Gia đình Bà Tư — Tiêu chí 26–35

> Phiên bản đã tích hợp mã nguồn vào **MiniSupermarket.API**. Hai project `MiniSupermarket.API` và `MiniSupermarket.WinForms` được giữ nguyên. Không tự động xóa/reseed database. **Chưa build hay kiểm thử trên SQL Server thực tế** trong môi trường chuẩn bị; người sử dụng phải kiểm thử trên Windows + SQL Server.

## 1. Chuẩn bị / chạy lần đầu

1. Giải nén ZIP, mở **`MiniSupperMakert-PhanNgocHieu.sln`** bằng Visual Studio 2022 trên Windows.
2. Xác nhận SQL Server đang chạy và tên DB trong `MiniSupermarket.API/appsettings.json` là `BachHoaGiaDinhBaTu_MigrationTestDb` (cấu hình đã giữ nguyên của file gốc). **Sao lưu database trước khi chạy các thử nghiệm tạo hóa đơn**.
3. Mở **Tools → NuGet Package Manager → Package Manager Console**. Chọn **Default project: `MiniSupermarket.API`**, thực hiện:

   ```powershell
   Update-Database
   ```

   Lệnh sẽ áp dụng migration `20261029190000_AddRevenueByShiftProcedure`, tạo/cập nhật `dbo.usp_RevenueByShift`. Không xóa các bản ghi sản phẩm/hóa đơn hiện có. API gốc kiểm tra *pending migrations* khi khởi động, vì thế **phải thực hiện bước này trước F5**. Nếu Visual Studio báo lỗi `Update-Database`, kiểm tra Default project, Startup project và kết nối SQL Server.
4. Build Solution (`Ctrl+Shift+B`), thiết lập `MiniSupermarket.API` làm Startup Project và F5; mở Swagger theo URL trong `launchSettings.json`, thường là `https://localhost:7237/swagger`.
5. Đăng nhập qua `POST /api/Auth/login`, sao chép token trả về và nhấn **Authorize** trong Swagger.
6. Nếu không muốn dùng migration, file SQL tham khảo nằm tại `MiniSupermarket.API/Database/TC34_RevenueByShift.sql`. Nhưng **chạy script SQL thủ công không đánh dấu migration đã áp dụng**, API vẫn yêu cầu chạy `Update-Database`. Khuyến nghị dùng migration, tránh làm hai lần.

**Mã vạch:** Database có thể đã đổi từ `BT000001` sang Barcode chỉ có số. Code trong bản này **không chỉnh lại mã seed**, không can thiệp vào Barcode hiện tại của bảng. Khi test, lấy giá trị Barcode thực tế bằng `SELECT ProductId, Barcode, ProductName FROM dbo.Products;` rồi dán chính xác vào Swagger (bao gồm số 0 đầu).

## 2. Vị trí của từng tiêu chí

| Tiêu chí | File / Phương thức cần trình bày | Kiểm chứng |
|---|---|---|
| 26 Include | `MiniSupermarket.API/Controllers/ProductsController.cs` — `GetAll()` | `Include(p => p.Category)` và `CategoryName` có giá trị |
| 27 AsNoTracking | `ProductsController.cs` — `GetAll()` và `BuildQuery()`, `CategoriesController.cs` | `AsNoTracking()` trong các GET đọc-only |
| 28 Đăng nhập JWT | `Controllers/AuthController.cs` — `Login()`, `GenerateJwtToken()` | Login 200 nhận token |
| 29 Khóa API | `ProductsController.cs`, `CategoriesController.cs` — `[Authorize]` | Gọi GET không kèm token phải được 401 |
| 30 Phân quyền | `[Authorize(Roles = "Admin,Warehouse")]` ở `ProductsController.cs` / `CategoriesController.cs`; `OrdersController.cs` role Cashier | Cashier POST Products → 403; Warehouse → 201 |
| 31 Swagger JWT | `MiniSupermarket.API/Program.cs` — `AddSwaggerGen()`, `UseAuthentication()` | Authorize xong gọi GET 200 |
| 32 ADO.NET tìm Barcode | `ProductsController.cs` — `GetByBarcode()` | `SqlConnection`, `SqlCommand`; `GET /api/Products/barcode/{barcode}` |
| 33 SqlDataReader + SQL injection | `ProductsController.cs` — `GetByBarcode()` | `SqlDataReader`, `command.Parameters.Add("@Barcode",...)`; chuỗi SQL injection không lấy dữ liệu |
| 34 Stored Procedure | `Database/TC34_RevenueByShift.sql`, `Migrations/20261029190000_AddRevenueByShiftProcedure.cs`, `Controllers/ShiftReportsController.cs` | `CommandType.StoredProcedure`, `GET /api/reports/shift` |
| 35 SqlTransaction / Rollback | `Controllers/OrdersController.cs` — `Checkout()` | `BeginTransaction(IsolationLevel.Serializable)`, `transaction.Rollback()`, `transaction.Commit()`, cập nhật đơn/chi tiết/kho/điểm trong cùng transaction |

## 3. Test Swagger

### TC26–27

- Gọi `GET /api/Products`: 200; từng phần tử có `categoryName` đúng với bảng `Categories`.
- Chụp source của `GetAll()`, phải thấy cả `.Include(p => p.Category)` và `.AsNoTracking()`.
- Gọi `GET /api/categories`: 200; chụp code `.AsNoTracking()`.

### TC28–31

- `POST /api/Auth/login`: đăng nhập tài khoản bạn đã tạo, ghi nhận JWT (`200`). **Không đưa mật khẩu hoặc token vào báo cáo công khai**.
- Thử `GET /api/Products` bằng một request **không có header Authorization**: `401`.
- Token Cashier: `POST /api/Products` phải `403`. Token Warehouse hoặc Admin mới có thể thêm sản phẩm. Sử dụng mã vạch thử nghiệm riêng để không trùng.
- Bấm Authorize trong Swagger với token phù hợp và gọi lại GET: `200`.

### TC32–33

- `GET /api/Products/barcode/{barcode}`: nhập Barcode đang tồn tại thực tế → `200`, đúng sản phẩm.
- Barcode không tồn tại → `404`.
- Barcode chứa payload thử nghiệm `' OR 1=1 --` (nhập ở Swagger): **không được lộ toàn bộ dữ liệu**; có thể `404` hoặc `400` tùy cách mã hóa URL.
- Chụp code có `SqlConnection`, `SqlCommand`, `ExecuteReaderAsync`, `@Barcode`.

### TC34

- `GET /api/reports/shift?date=2026-10-27&shift=3` với token Admin → `200`, doanh thu ca 22:00–06:00 theo giờ Việt Nam.
- `GET /api/reports/shift?date=2026-10-27&shift=0` → `400`.
- Dùng token Cashier → `403`.
- Đối chiếu cùng ngày/ca trong SSMS bằng:

  ```sql
  EXEC dbo.usp_RevenueByShift @ReportDate = '2026-10-27', @Shift = 3;
  ```

- **Chú ý:** `CreatedAt` đang lưu giờ UTC. Stored Procedure tính thời gian theo UTC+7; ca 3 được tính sang sáng hôm sau.

### TC35

Dùng môi trường database **kiểm thử**, sao lưu trước. POS WinForms vẫn gọi `POST /api/orders/checkout` như cũ; không phải sửa WinForms.

Trước khi test, kiểm tra sản phẩm, hóa đơn và điểm:

```sql
USE BachHoaGiaDinhBaTu_MigrationTestDb;
GO
SELECT ProductId, Barcode, ProductName, Price, StockQuantity
FROM dbo.Products WHERE ProductId = 1;
SELECT COUNT(*) AS TotalOrders FROM dbo.Orders;
SELECT CustomerId, PhoneNumber, RewardPoints FROM dbo.Customers;
```

**TC35-A (thanh toán thành công, tạo dữ liệu thật):** Sử dụng POS chọn sản phẩm ID 1, số lượng 1, tiền khách đưa đủ. Kiểm tra số hóa đơn +1, tồn kho giảm 1; nếu là thành viên thì điểm cộng `FLOOR(TotalAmount/10000)`.

**TC35-B (rollback khi thiếu tồn):** Gửi request với số lượng lớn hơn tồn kho (nhưng ≤ 10000), đơn giá đúng giá SQL. Ví dụ với giá 86000, nếu tồn kho dưới 10000, gửi:

```json
{
  "customerPhone": null,
  "cashReceived": 100000,
  "items": [{"productId":1,"quantity":10000,"unitPrice":86000}]
}
```

Kỳ vọng 400 (không đủ tồn kho), số đơn, số chi tiết, tồn kho và điểm **không đổi**. Giá thật cần lấy từ DB, vì API từ chối nếu giá đã thay đổi.

**TC35-C (khách không tồn tại):** Số lượng 1, tiền khách đủ, `customerPhone` đặt thành một số không tồn tại → `400` và database không đổi.

**TC35-D (thanh toán không đủ):** `cashReceived` thấp hơn giá trị hàng → `400`, database không đổi.

**TC35-E (hai món và một món không đủ tồn):** Hai sản phẩm khác nhau, một món tồn kho không đủ → không được có hóa đơn hoặc thay đổi số lượng nào. Đối chiếu trong `Orders`, `OrderDetails`, `Products`, `Customers`.

## 4. Ghi chú kỹ thuật

- `GetByBarcode()` chuyển từ EF LINQ sang ADO.NET nhưng **giữ nguyên đường dẫn cũ**, để màn hình POS hoạt động như trước.
- `Checkout()` chuyển từ EF Core transaction sang **`SqlTransaction` đúng yêu cầu TC35**, dùng parameterized SQL và khóa tồn kho khi đọc để tránh bán vượt trong môi trường nhiều quầy. Kết quả `CheckoutResponse` không thay đổi.
- Để chứng minh Rollback, chụp cả kết quả lỗi API và câu SELECT trước/sau trong SSMS.
- `appsettings.json` vẫn là file demo từ bản gốc; không công khai khóa JWT hoặc mật khẩu trên GitHub thật, cần dùng cấu hình bí mật phù hợp khi triển khai.
- **Không chạy `EnsureDeleted`, `EnsureCreated` hoặc seed lại DB** đang có dữ liệu bán hàng.
