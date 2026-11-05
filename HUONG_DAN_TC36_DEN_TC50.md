# BÁCH HÓA GIA ĐÌNH BÀ TƯ — HƯỚNG DẪN TC36–TC50

**Phạm vi:** ASP.NET Core 8 Web API, WinForms (.NET 8), SQL Server.

**Trạng thái:** Đã bổ sung source cho TC36–TC48 vào project đính kèm. TC49 và TC50 là hoạt động demo / vấn đáp, có kịch bản phía dưới. **Chưa xác nhận PASS hay chạy thực tế**: môi trường đóng gói không có .NET SDK/Visual Studio và SQL Server. Bạn cần Rebuild, chạy migration, kiểm thử và chụp ảnh trên Windows.

## 0. Cách chạy an toàn — BẮT BUỘC

1. **Sao lưu** `BachHoaGiaDinhBaTu_MigrationTestDb` bằng SQL Server Management Studio. **Đừng chạy** `EnsureDeleted`, `EnsureCreated` hay tạo lại database.
2. Giải nén. Mở `MiniSupperMakert-PhanNgocHieu.sln` bằng Visual Studio 2022 (có workload .NET desktop, ASP.NET Core và .NET 8 SDK).
3. Trong **Tools → NuGet Package Manager → Package Manager Console**, chọn Default project là **MiniSupermarket.API**, chạy `Update-Database` với kết nối trong `appsettings.json`. Lệnh sẽ thêm:
   - Cột `Customers.RegisteredAtUtc` (nullable: khách cũ sẽ có NULL, không bị tính là khách mới hôm nay).
   - Cột `Orders.DiscountAmount` (mặc định 0 cho hóa đơn cũ).
   - Stored Procedure `dbo.usp_RevenueTrend` (nếu migration đã chạy thì không chạy SQL tạo lại).
   - Stored Procedure TC34 phải được tạo trước từ migration cũ `20261029190000_AddRevenueByShiftProcedure`.
4. **Build → Rebuild Solution**. WinForms cần restore NuGet `WinForms.DataVisualization 1.10.2` (`Chart` cho .NET 8 Windows). Trang NuGet: https://www.nuget.org/packages/WinForms.DataVisualization/1.10.2
5. Chạy API (Swagger: `https://localhost:7237/swagger/index.html`); kiểm tra chuỗi `DefaultConnection` đang trỏ đúng `BachHoaGiaDinhBaTu_MigrationTestDb`.
6. Chạy WinForms, đăng nhập tài khoản Admin. Lưu ý demo account/mật khẩu chỉ sử dụng trong môi trường học tập; không đưa JWT/secret lên ảnh báo cáo.

**Nếu API báo `Database chua cap nhat Migrations`:** làm bước 3, không chỉnh `Program.cs` để bỏ kiểm tra.

## 1. Bảng vị trí code và minh chứng

| TC | Mở file / vị trí code | Chứng minh và kết quả mong đợi |
|---|---|---|
| 36 | `MiniSupermarket.WinForms/FormMainShell.Designer.cs` (`panelSidebar`, `panelTopHeader`, `panelMainContent`) | Chụp Shell có menu trái, header và vùng nội dung; form con không xuất hiện dưới dạng cửa sổ riêng. |
| 37 | `FormMainShell.cs`: `OpenChildForm`, `Navigate`; `BufferedPanel.cs` | Click nhanh luân phiên các menu, form không nháy trắng/không xuất hiện form nổi; `TopLevel=false`, `Dock=Fill`. |
| 38 | `FormLogin.cs`, `SessionManager.cs`: `LoginAsync` | Đăng nhập Admin, màn hình chính hiện đúng username và role; API `POST /api/Auth/login` trả token. |
| 39 | `FormMainShell.cs`: `CanAccess` / `ApplyRolePermissions` | Login Cashier chỉ có POS; Admin nhìn đủ menu; Warehouse thấy danh mục & sản phẩm. API vẫn phân quyền riêng. |
| 40 | `SessionManager.cs`: `ApiClientService.Client` | Có `HttpClient` tái sử dụng và `AuthenticationHeaderValue("Bearer", JwtToken)`; GET danh sách hoạt động. |
| 41 | `FormProductManagement.cs`: `SetupGrid`, `LoadProductsAsync`, `btnAdd_Click`, `btnUpdate_Click`, `btnDelete_Click` | Lưới hiển thị sản phẩm từ `GET /api/Products`; thử CRUD với bản ghi test, không dùng sản phẩm thực. |
| 42 | `FormPOS.cs`, `FormPOS.Payment.cs`: `txtBarcode_KeyDown`, `CalculateChange`, `btnCheckout_Click` | Quét/nhập barcode và Enter; cùng SP quét lại tăng số lượng; tiền thiếu hiển thị đỏ và báo âm thanh khi bấm thanh toán; tiền đủ mới tạo hóa đơn. |
| 43 | `FormProductManagement.cs`: `dgvProducts_CellFormatting` | Sản phẩm có `StockQuantity <= 10` được tô `LightCoral` / `DarkRed` tại cột Tồn kho. Lọc hoặc làm mới vẫn tô đúng. |
| 44 | API `Controllers/AnalyticsController.cs`: `DashboardSummary`; WinForms `FormAnalyticsDashboard.cs`: `MetricCard`, `LoadDashboardAsync`; Migration `AddCustomerRegistrationAndOrderDiscount` | Mở Báo cáo doanh thu (mặc định khi Admin đăng nhập), 4 thẻ: doanh thu hôm nay, tổng hóa đơn, KH mới hôm nay, mặt hàng tồn kho <= 10. Swagger `GET /api/reports/dashboard-summary`. |
| 45 | API `AnalyticsController.cs`: `RevenueTrend`; migration `AddRevenueTrendProcedure` chứa SQL GROUP BY; WinForms `FormAnalyticsDashboard.cs` (Chart) | Chọn Từ ngày, Đến ngày, nhóm day/week/month; biểu đồ cột đổi theo API `GET /api/reports/revenue-trend?period=day&from=2026-10-27&to=2026-11-04`. Nếu thiếu dữ liệu, hiển thị cột 0. |
| 46 | API `AnalyticsController.cs`: `TopProducts`; WinForms `FormAnalyticsDashboard.cs` (`_topGrid`) | `GET /api/reports/top-products?take=5` trả <= 5 nhóm `ProductId` đã bán, theo `SUM(Quantity)` giảm dần. Chụp Top 5 trên dashboard. |
| 47 | API `DTOs/CheckoutDtos.cs` (`UseRewardPoints`, `DiscountAmount`), `OrdersController.cs` (`Checkout`, `SqlTransaction`), WinForms `FormPOS.Payment.cs` (`_useRewardPoints`) | Chọn thành viên >= 50 điểm và hóa đơn > 50.000; tick "Dùng 50 điểm"; API trừ 50.000đ, trừ 50 điểm, tích điểm trên số tiền thanh toán trong cùng transaction. Nếu thiếu điểm hoặc thiếu kho: không có hóa đơn/điểm thay đổi. |
| 48 | API `AnalyticsController.cs`: `ExportProducts`, `ExportOrders`, `CsvFile`; WinForms `FormAnalyticsDashboard.cs`: `ExportCsvAsync` | Hai nút Xuất danh mục CSV, Xuất hóa đơn CSV. Chọn file bằng SaveFileDialog, mở bằng Excel (UTF-8 BOM). Swagger: `/api/reports/export/products.csv` và `/api/reports/export/orders.csv`. |
| 49 | Kịch bản demo mục 3 bên dưới | Tập trình diễn trực tiếp trên WinForms + SQL Server, **không thể tự đánh giá bằng code**. |
| 50 | Câu hỏi vấn đáp mục 4 bên dưới | Nắm đường đi UI → HttpClient → API JWT → SQL/EF → phản hồi, **không thể tự đánh giá bằng code**. |

## 2. Thông số kiểm thử trọng điểm

### TC43 — Cảnh báo tồn thấp

- Sao lưu giá trị `StockQuantity` của một bản ghi **thử nghiệm** (không nên dùng sản phẩm bán thực tế).
- Sử dụng API Admin/Warehouse `PUT /api/Products/{id}` đặt `StockQuantity=10` ⇒ ô tồn kho màu đỏ/cam.
- Đổi thành `11` ⇒ ô trở về bình thường. Khôi phục giá trị ban đầu.

### TC44 — Dashboard

- `GET /api/reports/dashboard-summary`: HTTP `200` khi Admin có token; gồm `todayRevenue`, `totalInvoices`, `newCustomersToday`, `lowStockProducts`.
- Chú ý: khách có sẵn từ trước khi nâng cấp không có mốc đăng ký nên **không được tính là "khách mới hôm nay"**. Thử tạo khách mới bằng `POST /api/customers`, làm mới Dashboard, số mới tăng lên 1.
- Hóa đơn chỉ tính doanh thu **sau giảm**; `Orders.CreatedAt` lưu UTC, báo cáo chuyển theo Việt Nam UTC+7.

### TC45 — Doanh thu ngày/tuần/tháng

- Swagger `GET /api/reports/revenue-trend?period=day&from=2026-10-27&to=2026-10-28` ⇒ trả dữ liệu mỗi ngày, `200`.
- `period=week` ⇒ nhóm Thứ Hai–Chủ Nhật; `period=month` ⇒ nhóm tháng; SQL Stored Procedure dùng `GROUP BY BucketDate`.
- `from > to` ⇒ `400`; khoảng quá dài ⇒ `400`.

### TC46 — Top bán chạy

- `GET /api/reports/top-products?take=5`, `200`, danh sách <= 5 phần tử.
- So sánh SQL: `SELECT TOP 5 ProductId, SUM(Quantity) AS SL FROM dbo.OrderDetails GROUP BY ProductId ORDER BY SL DESC;`

### TC47 — Dùng 50 điểm (test trên database backup hoặc test)

- Khách có 120 điểm; hóa đơn giá gốc 86.000đ. Bật đổi 50 điểm: tiền phải trả = 36.000đ.
- Điểm sau thanh toán = `120 - 50 + FLOOR(36000/10000) = 73` (tích 1 điểm mỗi 10.000đ thanh toán).
- Hóa đơn lưu `TotalAmount=36000`, `DiscountAmount=50000`, `CashReceived` là tiền đưa thực tế.
- Trường hợp khách chỉ 49 điểm / không xác minh khách / hóa đơn <= 50.000đ: **không cho đổi điểm**.
- Test rollback: tạo hóa đơn gồm 1 mã hàng còn và 1 mã hết hàng; gửi `UseRewardPoints=true`. Xác minh `Orders`, `OrderDetails`, `Products.StockQuantity`, `Customers.RewardPoints` đều không bị thay đổi vì giao dịch thất bại.
- Để tránh bán vượt kho: API dùng `UPDLOCK,HOLDLOCK` và `UPDATE ... WHERE StockQuantity >= @Quantity` trong `SqlTransaction`.

### TC48 — Export

- Đăng nhập Admin → menu Báo cáo → Xuất danh mục CSV hoặc Xuất hóa đơn CSV → chọn Save → mở trong Excel.
- Kiểm tra đủ dòng; Unicode tiếng Việt đúng; Barcode có số 0 đầu được giữ khi xử lý dạng *text* (Excel có thể tự chuyển kiểu khi mở CSV trực tiếp — muốn chắc chắn, dùng Excel → Data → From Text/CSV và đặt Barcode là Text).
- Lưu ý đầu ra **CSV chứ không phải XLSX**; đề bài cho phép CSV hoặc XLSX. Báo cáo thuộc dữ liệu quản trị nên chỉ Admin tải được.

## 3. TC49 — Kịch bản demo 3–5 phút trước thầy

1. Đăng nhập Admin → chỉ rõ header tên nhân viên / vai trò và Dashboard 4 KPI.
2. Chọn phạm vi báo cáo, chuyển biểu đồ theo ngày → tuần → tháng; xem Top 5.
3. Mở Sản phẩm → chỉ mã vạch, danh mục, số tồn; chỉ màu tồn <= 10 (dùng một dữ liệu test).
4. Đăng xuất Admin → đăng nhập Cashier → chứng minh chỉ thấy menu POS.
5. Quét barcode / Enter hai lần → số lượng 2; nhập tiền thiếu → cảnh báo; đủ tiền → thanh toán.
6. Chọn khách thành viên đủ điểm và đơn trên 50.000 → tích "Dùng 50 điểm trừ 50.000" → hoàn tất thanh toán; kiểm tra SQL tồn kho và điểm.
7. Đăng nhập lại Admin → xuất CSV; chỉ kết quả trong Excel hoặc thư mục lưu.

**Chuẩn bị:** SQL Server và API chạy sẵn; có một sản phẩm test đủ kho, một sản phẩm test sắp hết kho; một khách thành viên đủ điểm; sao lưu DB. **Không dùng bản ghi thật để test DELETE hoặc gây giảm tồn kho ngoài ý muốn.**

## 4. TC50 — Những câu trả lời khi bảo vệ

- **Tại sao dùng Shell?** Form con nằm trong `panelMainContent`, `TopLevel=false`, `Dock=Fill`, menu/header cố định.
- **JWT được lưu ở đâu?** `SessionManager.JwtToken`, `ApiClientService.Client` tự đưa vào Authorization Bearer của request; API kiểm tra chữ ký/thời hạn/role.
- **Tại sao vừa ẩn nút vừa [Authorize]?** Ẩn menu chỉ là trải nghiệm UI, phân quyền thực sự bắt buộc ở API.
- **Vì sao dùng AsNoTracking?** Query GET chỉ đọc không cần EF theo dõi thay đổi.
- **Tại sao báo doanh thu khác giá gốc chi tiết?** `Orders.TotalAmount` lưu số khách trả sau giảm điểm, `Orders.DiscountAmount` lưu giảm giá; `OrderDetails` lưu giá gốc và số lượng bán.
- **Nếu hai quầy bán cuối cùng một sản phẩm?** SQL transaction với lock và điều kiện tồn kho; giao dịch không đủ kho sẽ Rollback.
- **CSV khác XLSX thế nào?** CSV là văn bản bảng UTF-8, mở bằng Excel; không chứa định dạng/sheet phức tạp như XLSX.
- **Khách mới hôm nay tính thế nào?** Cột `RegisteredAtUtc` thêm ở migration, đánh dấu đúng lúc tạo khách; khách cũ để NULL nên không tính nhầm.
- **Dữ liệu dùng để vẽ chart đến từ đâu?** Stored Procedure `usp_RevenueTrend` GROUP BY trên SQL Server, API trả JSON, WinForms đổ vào `Chart.Series` kiểu Column.

## 5. Trước khi báo cáo hãy chụp minh chứng

- TC36: Shell đầy đủ 3 vùng; TC37: cùng 1 cửa sổ khi click menu.
- TC38: Login thành công; TC39: Cashier chỉ thấy POS; TC40: Code HttpClient + Bearer.
- TC41: DataGridView CRUD; TC42: POS tiền đủ/thiếu; TC43: tô màu mặt hàng tồn <=10.
- TC44: 4 KPI; TC45: Bar Chart thay đổi ngày/tuần/tháng; TC46: top 5.
- TC47: ảnh checkbox đổi điểm + kết quả API/SQL; TC48: file CSV mở Excel.
- TC49: ảnh toàn màn hình đang demo; TC50: kịch bản Q&A và video trình diễn nếu thầy yêu cầu.

**Rất quan trọng:** Cài đặt `JwtSettings:Secret` và tài khoản demo trong source này chỉ phù hợp môi trường môn học. Không đưa source có khóa bí mật công khai lên repository public, không chụp token/bearer đầy đủ vào báo cáo.
