using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MembershipRank = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RewardPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CashierUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CashReceived = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Gạo và ngũ cốc", "Gạo các loại, yến mạch và ngũ cốc ăn sáng" },
                    { 2, "Mì, bún và phở", "Mì gói, phở ăn liền và bún khô" },
                    { 3, "Dầu ăn và gia vị", "Dầu ăn, nước mắm, nước tương và gia vị nấu ăn" },
                    { 4, "Đồ hộp và thực phẩm khô", "Cá hộp, thịt hộp, đồ khô và thực phẩm dự trữ" },
                    { 5, "Thịt, cá và hải sản", "Thực phẩm tươi sống phục vụ bữa ăn gia đình" },
                    { 6, "Trứng và thực phẩm đông lạnh", "Trứng, xúc xích và đồ ăn đông lạnh" },
                    { 7, "Rau củ tươi", "Rau lá, củ và quả dùng chế biến món ăn" },
                    { 8, "Trái cây tươi", "Trái cây theo mùa và trái cây nhập khẩu" },
                    { 9, "Sữa và chế phẩm từ sữa", "Sữa tươi, sữa đặc và sữa chua" },
                    { 10, "Nước uống và giải khát", "Nước suối, trà đóng chai và nước ngọt" },
                    { 11, "Bánh kẹo và đồ ăn vặt", "Bánh quy, snack và các loại kẹo" },
                    { 12, "Chăm sóc cá nhân", "Dầu gội, kem đánh răng và sữa tắm" },
                    { 13, "Hóa phẩm và vệ sinh", "Bột giặt, nước rửa chén và chất lau sàn" },
                    { 14, "Giấy và đồ gia dụng", "Giấy vệ sinh, khăn giấy và túi rác" },
                    { 15, "Thực phẩm tiện lợi", "Cháo gói, bánh mì và món ăn nhanh đóng gói" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 1, "25 Nguyễn Huệ, Phường Bến Nghé, TP. Hồ Chí Minh", "Nguyễn Văn An", "Bạc", "0901234567", 120 },
                    { 2, "148 Võ Văn Tần, Phường Võ Thị Sáu, TP. Hồ Chí Minh", "Trần Thị Bình", "Chuẩn", "0912345678", 50 },
                    { 3, "72 Nguyễn Thị Thập, Phường Tân Quy, TP. Hồ Chí Minh", "Lê Minh Cường", "Vàng", "0923456789", 350 },
                    { 4, "186 Võ Văn Ngân, Phường Bình Thọ, TP. Hồ Chí Minh", "Phạm Thị Dung", "Chuẩn", "0934567890", 20 },
                    { 5, "42 Điện Biên Phủ, Phường 22, TP. Hồ Chí Minh", "Hoàng Minh Đức", "Kim cương", "0945678901", 800 },
                    { 6, "315 Cộng Hòa, Phường 13, TP. Hồ Chí Minh", "Võ Thị Hạnh", "Bạc", "0956789012", 180 },
                    { 7, "89 Quang Trung, Phường 10, TP. Hồ Chí Minh", "Đặng Quốc Huy", "Vàng", "0967890123", 420 },
                    { 8, "156 Thành Thái, Phường 12, TP. Hồ Chí Minh", "Bùi Ngọc Lan", "Chuẩn", "0978901234", 75 },
                    { 9, "214 Nguyễn Trãi, Phường 3, TP. Hồ Chí Minh", "Phan Thanh Long", "Bạc", "0989012345", 260 },
                    { 10, "58 Lạc Long Quân, Phường 10, TP. Hồ Chí Minh", "Ngô Thị Mai", "Vàng", "0990123456", 620 },
                    { 11, "127 Phan Đình Phùng, Phường 17, TP. Hồ Chí Minh", "Đỗ Văn Nam", "Chuẩn", "0881234567", 15 },
                    { 12, "96 Hậu Giang, Phường 11, TP. Hồ Chí Minh", "Vũ Thị Oanh", "Bạc", "0872345678", 290 },
                    { 13, "235 Tỉnh Lộ 10, Phường Bình Trị Đông, TP. Hồ Chí Minh", "Đinh Minh Phúc", "Kim cương", "0863456789", 950 },
                    { 14, "74 Lũy Bán Bích, Phường Tân Thành, TP. Hồ Chí Minh", "Mai Thị Quỳnh", "Bạc", "0854567890", 110 },
                    { 15, "318 Nguyễn Văn Quá, Phường Đông Hưng Thuận, TP. Hồ Chí Minh", "Trương Hoàng Sơn", "Chuẩn", "0845678901", 45 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FullName", "IsActive", "Password", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Nguyễn Quản Trị", false, "123456", "", "Admin", "admin01" },
                    { 2, "Trần Giám Đốc", false, "123456", "", "Admin", "admin02" },
                    { 3, "Lê Thu Ngân", false, "123456", "", "Cashier", "cashier01" },
                    { 4, "Phạm Bán Hàng", false, "123456", "", "Cashier", "cashier02" },
                    { 5, "Hoàng Thu Ngân", false, "123456", "", "Cashier", "cashier03" },
                    { 6, "Vũ Thị Quầy", false, "123456", "", "Cashier", "cashier04" },
                    { 7, "Đỗ Bán Lẻ", false, "123456", "", "Cashier", "cashier05" },
                    { 8, "Ngô Quản Kho", false, "123456", "", "Warehouse", "ware01" },
                    { 9, "Bùi Kiểm Kê", false, "123456", "", "Warehouse", "ware02" },
                    { 10, "Dương Thủ Kho", false, "123456", "", "Warehouse", "ware03" },
                    { 11, "Lý Nhập Hàng", false, "123456", "", "Warehouse", "ware04" },
                    { 12, "Đặng Hỗ Trợ", false, "123456", "", "Admin", "admin_backup" },
                    { 13, "Hồ Ca Chiều", false, "123456", "", "Cashier", "cashier06" },
                    { 14, "Trương Vận Chuyển", false, "123456", "", "Warehouse", "ware05" },
                    { 15, "Mai Giám Sát", false, "123456", "", "Admin", "supervisor" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "BT000001", 1, 86000m, "Gạo ST25 túi 2kg", 46 },
                    { 2, "BT000002", 1, 145000m, "Gạo Jasmine túi 5kg", 35 },
                    { 3, "BT000003", 1, 42000m, "Yến mạch cán dẹt 500g", 50 },
                    { 4, "BT000004", 2, 4500m, "Mì Hảo Hảo tôm chua cay 75g", 120 },
                    { 5, "BT000005", 2, 9500m, "Phở bò ăn liền 65g", 90 },
                    { 6, "BT000006", 2, 26000m, "Bún gạo khô 400g", 65 },
                    { 7, "BT000007", 3, 55000m, "Dầu ăn Tường An 1L", 45 },
                    { 8, "BT000008", 3, 26000m, "Nước mắm Nam Ngư 500ml", 70 },
                    { 9, "BT000009", 3, 23000m, "Nước tương Chinsu 500ml", 80 },
                    { 10, "BT000010", 4, 38000m, "Cá ngừ đóng hộp 170g", 40 },
                    { 11, "BT000011", 4, 57000m, "Thịt hộp 340g", 30 },
                    { 12, "BT000012", 4, 45000m, "Nấm hương khô 100g", 48 },
                    { 13, "BT000013", 5, 85000m, "Thịt ba rọi heo 500g", 36 },
                    { 14, "BT000014", 5, 55000m, "Ức gà 500g", 46 },
                    { 15, "BT000015", 5, 49000m, "Cá basa phi lê 500g", 38 },
                    { 16, "BT000016", 6, 32000m, "Trứng gà hộp 10 quả", 60 },
                    { 17, "BT000017", 6, 55000m, "Xúc xích tươi gói 500g", 38 },
                    { 18, "BT000018", 6, 69000m, "Há cảo tôm đông lạnh 500g", 32 },
                    { 19, "BT000019", 7, 16000m, "Cà rốt 500g", 55 },
                    { 20, "BT000020", 7, 22000m, "Cà chua 500g", 52 },
                    { 21, "BT000021", 7, 18000m, "Xà lách 300g", 40 },
                    { 22, "BT000022", 8, 36000m, "Cam sành 1kg", 42 },
                    { 23, "BT000023", 8, 76000m, "Táo Fuji 1kg", 34 },
                    { 24, "BT000024", 8, 30000m, "Chuối già 1kg", 48 },
                    { 25, "BT000025", 9, 36000m, "Sữa tươi TH true MILK 1L", 80 },
                    { 26, "BT000026", 9, 27000m, "Sữa đặc Ông Thọ 380g", 70 },
                    { 27, "BT000027", 9, 32000m, "Sữa chua Vinamilk lốc 4 hộp", 65 },
                    { 28, "BT000028", 10, 6000m, "Nước suối La Vie 500ml", 150 },
                    { 29, "BT000029", 10, 11000m, "Trà xanh Không Độ 500ml", 120 },
                    { 30, "BT000030", 10, 12000m, "Coca-Cola lon 330ml", 110 },
                    { 31, "BT000031", 11, 39000m, "Bánh quy Cosy 300g", 60 },
                    { 32, "BT000032", 11, 14000m, "Snack Oishi 60g", 95 },
                    { 33, "BT000033", 11, 19000m, "Kẹo dẻo trái cây 100g", 62 },
                    { 34, "BT000034", 12, 79000m, "Dầu gội Clear 340ml", 44 },
                    { 35, "BT000035", 12, 42000m, "Kem đánh răng Colgate 180g", 55 },
                    { 36, "BT000036", 12, 99000m, "Sữa tắm Lifebuoy 800g", 33 },
                    { 37, "BT000037", 13, 31000m, "Nước rửa chén Sunlight 750ml", 65 },
                    { 38, "BT000038", 13, 117000m, "Bột giặt OMO 2,7kg", 38 },
                    { 39, "BT000039", 13, 28000m, "Nước lau sàn Gift 1L", 47 },
                    { 40, "BT000040", 14, 49000m, "Giấy vệ sinh lốc 10 cuộn", 53 },
                    { 41, "BT000041", 14, 24000m, "Khăn giấy hộp 180 tờ", 80 },
                    { 42, "BT000042", 14, 23000m, "Túi rác cuộn bộ 3", 64 },
                    { 43, "BT000043", 15, 29000m, "Bánh mì sandwich 450g", 44 },
                    { 44, "BT000044", 15, 8000m, "Cháo ăn liền gói 50g", 90 },
                    { 45, "BT000045", 15, 26000m, "Cơm cháy chà bông 120g", 56 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
