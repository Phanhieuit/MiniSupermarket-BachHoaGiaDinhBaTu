using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(
            DbContextOptions<SupermarketDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");
            modelBuilder.Entity<Order>().HasMany(o => o.OrderDetails).WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany()
                .HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>().Property(o => o.CashReceived).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.DiscountAmount).HasPrecision(18, 2).HasDefaultValue(0m);
            modelBuilder.Entity<OrderDetail>().Property(o => o.UnitPrice).HasPrecision(18, 2);

            // =====================================================
            // QUAN HỆ CATEGORY - PRODUCT
            // 1 Category có nhiều Product
            // =====================================================

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // SEED CATEGORY
            // =====================================================

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Gạo và ngũ cốc", Description = "Gạo các loại, yến mạch và ngũ cốc ăn sáng" },
                new Category { CategoryId = 2, CategoryName = "Mì, bún và phở", Description = "Mì gói, phở ăn liền và bún khô" },
                new Category { CategoryId = 3, CategoryName = "Dầu ăn và gia vị", Description = "Dầu ăn, nước mắm, nước tương và gia vị nấu ăn" },
                new Category { CategoryId = 4, CategoryName = "Đồ hộp và thực phẩm khô", Description = "Cá hộp, thịt hộp, đồ khô và thực phẩm dự trữ" },
                new Category { CategoryId = 5, CategoryName = "Thịt, cá và hải sản", Description = "Thực phẩm tươi sống phục vụ bữa ăn gia đình" },
                new Category { CategoryId = 6, CategoryName = "Trứng và thực phẩm đông lạnh", Description = "Trứng, xúc xích và đồ ăn đông lạnh" },
                new Category { CategoryId = 7, CategoryName = "Rau củ tươi", Description = "Rau lá, củ và quả dùng chế biến món ăn" },
                new Category { CategoryId = 8, CategoryName = "Trái cây tươi", Description = "Trái cây theo mùa và trái cây nhập khẩu" },
                new Category { CategoryId = 9, CategoryName = "Sữa và chế phẩm từ sữa", Description = "Sữa tươi, sữa đặc và sữa chua" },
                new Category { CategoryId = 10, CategoryName = "Nước uống và giải khát", Description = "Nước suối, trà đóng chai và nước ngọt" },
                new Category { CategoryId = 11, CategoryName = "Bánh kẹo và đồ ăn vặt", Description = "Bánh quy, snack và các loại kẹo" },
                new Category { CategoryId = 12, CategoryName = "Chăm sóc cá nhân", Description = "Dầu gội, kem đánh răng và sữa tắm" },
                new Category { CategoryId = 13, CategoryName = "Hóa phẩm và vệ sinh", Description = "Bột giặt, nước rửa chén và chất lau sàn" },
                new Category { CategoryId = 14, CategoryName = "Giấy và đồ gia dụng", Description = "Giấy vệ sinh, khăn giấy và túi rác" },
                new Category { CategoryId = 15, CategoryName = "Thực phẩm tiện lợi", Description = "Cháo gói, bánh mì và món ăn nhanh đóng gói" }
            );

            // =====================================================
            // SEED PRODUCT
            // =====================================================

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Barcode = "BT000001", ProductName = "Gạo ST25 túi 2kg", Price = 86000, StockQuantity = 46, CategoryId = 1 },
                new Product { ProductId = 2, Barcode = "BT000002", ProductName = "Gạo Jasmine túi 5kg", Price = 145000, StockQuantity = 35, CategoryId = 1 },
                new Product { ProductId = 3, Barcode = "BT000003", ProductName = "Yến mạch cán dẹt 500g", Price = 42000, StockQuantity = 50, CategoryId = 1 },
                new Product { ProductId = 4, Barcode = "BT000004", ProductName = "Mì Hảo Hảo tôm chua cay 75g", Price = 4500, StockQuantity = 120, CategoryId = 2 },
                new Product { ProductId = 5, Barcode = "BT000005", ProductName = "Phở bò ăn liền 65g", Price = 9500, StockQuantity = 90, CategoryId = 2 },
                new Product { ProductId = 6, Barcode = "BT000006", ProductName = "Bún gạo khô 400g", Price = 26000, StockQuantity = 65, CategoryId = 2 },
                new Product { ProductId = 7, Barcode = "BT000007", ProductName = "Dầu ăn Tường An 1L", Price = 55000, StockQuantity = 45, CategoryId = 3 },
                new Product { ProductId = 8, Barcode = "BT000008", ProductName = "Nước mắm Nam Ngư 500ml", Price = 26000, StockQuantity = 70, CategoryId = 3 },
                new Product { ProductId = 9, Barcode = "BT000009", ProductName = "Nước tương Chinsu 500ml", Price = 23000, StockQuantity = 80, CategoryId = 3 },
                new Product { ProductId = 10, Barcode = "BT000010", ProductName = "Cá ngừ đóng hộp 170g", Price = 38000, StockQuantity = 40, CategoryId = 4 },
                new Product { ProductId = 11, Barcode = "BT000011", ProductName = "Thịt hộp 340g", Price = 57000, StockQuantity = 30, CategoryId = 4 },
                new Product { ProductId = 12, Barcode = "BT000012", ProductName = "Nấm hương khô 100g", Price = 45000, StockQuantity = 48, CategoryId = 4 },
                new Product { ProductId = 13, Barcode = "BT000013", ProductName = "Thịt ba rọi heo 500g", Price = 85000, StockQuantity = 36, CategoryId = 5 },
                new Product { ProductId = 14, Barcode = "BT000014", ProductName = "Ức gà 500g", Price = 55000, StockQuantity = 46, CategoryId = 5 },
                new Product { ProductId = 15, Barcode = "BT000015", ProductName = "Cá basa phi lê 500g", Price = 49000, StockQuantity = 38, CategoryId = 5 },
                new Product { ProductId = 16, Barcode = "BT000016", ProductName = "Trứng gà hộp 10 quả", Price = 32000, StockQuantity = 60, CategoryId = 6 },
                new Product { ProductId = 17, Barcode = "BT000017", ProductName = "Xúc xích tươi gói 500g", Price = 55000, StockQuantity = 38, CategoryId = 6 },
                new Product { ProductId = 18, Barcode = "BT000018", ProductName = "Há cảo tôm đông lạnh 500g", Price = 69000, StockQuantity = 32, CategoryId = 6 },
                new Product { ProductId = 19, Barcode = "BT000019", ProductName = "Cà rốt 500g", Price = 16000, StockQuantity = 55, CategoryId = 7 },
                new Product { ProductId = 20, Barcode = "BT000020", ProductName = "Cà chua 500g", Price = 22000, StockQuantity = 52, CategoryId = 7 },
                new Product { ProductId = 21, Barcode = "BT000021", ProductName = "Xà lách 300g", Price = 18000, StockQuantity = 40, CategoryId = 7 },
                new Product { ProductId = 22, Barcode = "BT000022", ProductName = "Cam sành 1kg", Price = 36000, StockQuantity = 42, CategoryId = 8 },
                new Product { ProductId = 23, Barcode = "BT000023", ProductName = "Táo Fuji 1kg", Price = 76000, StockQuantity = 34, CategoryId = 8 },
                new Product { ProductId = 24, Barcode = "BT000024", ProductName = "Chuối già 1kg", Price = 30000, StockQuantity = 48, CategoryId = 8 },
                new Product { ProductId = 25, Barcode = "BT000025", ProductName = "Sữa tươi TH true MILK 1L", Price = 36000, StockQuantity = 80, CategoryId = 9 },
                new Product { ProductId = 26, Barcode = "BT000026", ProductName = "Sữa đặc Ông Thọ 380g", Price = 27000, StockQuantity = 70, CategoryId = 9 },
                new Product { ProductId = 27, Barcode = "BT000027", ProductName = "Sữa chua Vinamilk lốc 4 hộp", Price = 32000, StockQuantity = 65, CategoryId = 9 },
                new Product { ProductId = 28, Barcode = "BT000028", ProductName = "Nước suối La Vie 500ml", Price = 6000, StockQuantity = 150, CategoryId = 10 },
                new Product { ProductId = 29, Barcode = "BT000029", ProductName = "Trà xanh Không Độ 500ml", Price = 11000, StockQuantity = 120, CategoryId = 10 },
                new Product { ProductId = 30, Barcode = "BT000030", ProductName = "Coca-Cola lon 330ml", Price = 12000, StockQuantity = 110, CategoryId = 10 },
                new Product { ProductId = 31, Barcode = "BT000031", ProductName = "Bánh quy Cosy 300g", Price = 39000, StockQuantity = 60, CategoryId = 11 },
                new Product { ProductId = 32, Barcode = "BT000032", ProductName = "Snack Oishi 60g", Price = 14000, StockQuantity = 95, CategoryId = 11 },
                new Product { ProductId = 33, Barcode = "BT000033", ProductName = "Kẹo dẻo trái cây 100g", Price = 19000, StockQuantity = 62, CategoryId = 11 },
                new Product { ProductId = 34, Barcode = "BT000034", ProductName = "Dầu gội Clear 340ml", Price = 79000, StockQuantity = 44, CategoryId = 12 },
                new Product { ProductId = 35, Barcode = "BT000035", ProductName = "Kem đánh răng Colgate 180g", Price = 42000, StockQuantity = 55, CategoryId = 12 },
                new Product { ProductId = 36, Barcode = "BT000036", ProductName = "Sữa tắm Lifebuoy 800g", Price = 99000, StockQuantity = 33, CategoryId = 12 },
                new Product { ProductId = 37, Barcode = "BT000037", ProductName = "Nước rửa chén Sunlight 750ml", Price = 31000, StockQuantity = 65, CategoryId = 13 },
                new Product { ProductId = 38, Barcode = "BT000038", ProductName = "Bột giặt OMO 2,7kg", Price = 117000, StockQuantity = 38, CategoryId = 13 },
                new Product { ProductId = 39, Barcode = "BT000039", ProductName = "Nước lau sàn Gift 1L", Price = 28000, StockQuantity = 47, CategoryId = 13 },
                new Product { ProductId = 40, Barcode = "BT000040", ProductName = "Giấy vệ sinh lốc 10 cuộn", Price = 49000, StockQuantity = 53, CategoryId = 14 },
                new Product { ProductId = 41, Barcode = "BT000041", ProductName = "Khăn giấy hộp 180 tờ", Price = 24000, StockQuantity = 80, CategoryId = 14 },
                new Product { ProductId = 42, Barcode = "BT000042", ProductName = "Túi rác cuộn bộ 3", Price = 23000, StockQuantity = 64, CategoryId = 14 },
                new Product { ProductId = 43, Barcode = "BT000043", ProductName = "Bánh mì sandwich 450g", Price = 29000, StockQuantity = 44, CategoryId = 15 },
                new Product { ProductId = 44, Barcode = "BT000044", ProductName = "Cháo ăn liền gói 50g", Price = 8000, StockQuantity = 90, CategoryId = 15 },
                new Product { ProductId = 45, Barcode = "BT000045", ProductName = "Cơm cháy chà bông 120g", Price = 26000, StockQuantity = 56, CategoryId = 15 }
            );
            // =========================
            // CUSTOMER
            // =========================
            
            modelBuilder.Entity<Customer>().HasData(

                new Customer
                {
                    CustomerId = 1,
                    CustomerName = "Nguyễn Văn An",
                    PhoneNumber = "0901234567",
                    Address = "25 Nguyễn Huệ, Phường Bến Nghé, TP. Hồ Chí Minh",
                    RewardPoints = 120,
                    MembershipRank = "Bạc"
                },

                new Customer
                {
                    CustomerId = 2,
                    CustomerName = "Trần Thị Bình",
                    PhoneNumber = "0912345678",
                    Address = "148 Võ Văn Tần, Phường Võ Thị Sáu, TP. Hồ Chí Minh",
                    RewardPoints = 50,
                    MembershipRank = "Chuẩn"
                },

                new Customer
                {
                    CustomerId = 3,
                    CustomerName = "Lê Minh Cường",
                    PhoneNumber = "0923456789",
                    Address = "72 Nguyễn Thị Thập, Phường Tân Quy, TP. Hồ Chí Minh",
                    RewardPoints = 350,
                    MembershipRank = "Vàng"
                },

                new Customer
                {
                    CustomerId = 4,
                    CustomerName = "Phạm Thị Dung",
                    PhoneNumber = "0934567890",
                    Address = "186 Võ Văn Ngân, Phường Bình Thọ, TP. Hồ Chí Minh",
                    RewardPoints = 20,
                    MembershipRank = "Chuẩn"
                },

                new Customer
                {
                    CustomerId = 5,
                    CustomerName = "Hoàng Minh Đức",
                    PhoneNumber = "0945678901",
                    Address = "42 Điện Biên Phủ, Phường 22, TP. Hồ Chí Minh",
                    RewardPoints = 800,
                    MembershipRank = "Kim cương"
                },

                new Customer
                {
                    CustomerId = 6,
                    CustomerName = "Võ Thị Hạnh",
                    PhoneNumber = "0956789012",
                    Address = "315 Cộng Hòa, Phường 13, TP. Hồ Chí Minh",
                    RewardPoints = 180,
                    MembershipRank = "Bạc"
                },

                new Customer
                {
                    CustomerId = 7,
                    CustomerName = "Đặng Quốc Huy",
                    PhoneNumber = "0967890123",
                    Address = "89 Quang Trung, Phường 10, TP. Hồ Chí Minh",
                    RewardPoints = 420,
                    MembershipRank = "Vàng"
                },

                new Customer
                {
                    CustomerId = 8,
                    CustomerName = "Bùi Ngọc Lan",
                    PhoneNumber = "0978901234",
                    Address = "156 Thành Thái, Phường 12, TP. Hồ Chí Minh",
                    RewardPoints = 75,
                    MembershipRank = "Chuẩn"
                },

                new Customer
                {
                    CustomerId = 9,
                    CustomerName = "Phan Thanh Long",
                    PhoneNumber = "0989012345",
                    Address = "214 Nguyễn Trãi, Phường 3, TP. Hồ Chí Minh",
                    RewardPoints = 260,
                    MembershipRank = "Bạc"
                },

                new Customer
                {
                    CustomerId = 10,
                    CustomerName = "Ngô Thị Mai",
                    PhoneNumber = "0990123456",
                    Address = "58 Lạc Long Quân, Phường 10, TP. Hồ Chí Minh",
                    RewardPoints = 620,
                    MembershipRank = "Vàng"
                },

                new Customer
                {
                    CustomerId = 11,
                    CustomerName = "Đỗ Văn Nam",
                    PhoneNumber = "0881234567",
                    Address = "127 Phan Đình Phùng, Phường 17, TP. Hồ Chí Minh",
                    RewardPoints = 15,
                    MembershipRank = "Chuẩn"
                },

                new Customer
                {
                    CustomerId = 12,
                    CustomerName = "Vũ Thị Oanh",
                    PhoneNumber = "0872345678",
                    Address = "96 Hậu Giang, Phường 11, TP. Hồ Chí Minh",
                    RewardPoints = 290,
                    MembershipRank = "Bạc"
                },

                new Customer
                {
                    CustomerId = 13,
                    CustomerName = "Đinh Minh Phúc",
                    PhoneNumber = "0863456789",
                    Address = "235 Tỉnh Lộ 10, Phường Bình Trị Đông, TP. Hồ Chí Minh",
                    RewardPoints = 950,
                    MembershipRank = "Kim cương"
                },

                new Customer
                {
                    CustomerId = 14,
                    CustomerName = "Mai Thị Quỳnh",
                    PhoneNumber = "0854567890",
                    Address = "74 Lũy Bán Bích, Phường Tân Thành, TP. Hồ Chí Minh",
                    RewardPoints = 110,
                    MembershipRank = "Bạc"
                },

                new Customer
                {
                    CustomerId = 15,
                    CustomerName = "Trương Hoàng Sơn",
                    PhoneNumber = "0845678901",
                    Address = "318 Nguyễn Văn Quá, Phường Đông Hưng Thuận, TP. Hồ Chí Minh",
                    RewardPoints = 45,
                    MembershipRank = "Chuẩn"
                }
            );

            modelBuilder.Entity<User>().HasData(
        new User
        {
            UserId = 1,
            Username = "admin01",
            Password = "123456",
            FullName = "Nguyễn Quản Trị",
            Role = "Admin"
        },

        new User
        {
            UserId = 2,
            Username = "admin02",
            Password = "123456",
            FullName = "Trần Giám Đốc",
            Role = "Admin"
        },

        new User
        {
            UserId = 3,
            Username = "cashier01",
            Password = "123456",
            FullName = "Lê Thu Ngân",
            Role = "Cashier"
        },

        new User
        {
            UserId = 4,
            Username = "cashier02",
            Password = "123456",
            FullName = "Phạm Bán Hàng",
            Role = "Cashier"
        },

        new User
        {
            UserId = 5,
            Username = "cashier03",
            Password = "123456",
            FullName = "Hoàng Thu Ngân",
            Role = "Cashier"
        },

        new User
        {
            UserId = 6,
            Username = "cashier04",
            Password = "123456",
            FullName = "Vũ Thị Quầy",
            Role = "Cashier"
        },

        new User
        {
            UserId = 7,
            Username = "cashier05",
            Password = "123456",
            FullName = "Đỗ Bán Lẻ",
            Role = "Cashier"
        },

        new User
        {
            UserId = 8,
            Username = "ware01",
            Password = "123456",
            FullName = "Ngô Quản Kho",
            Role = "Warehouse"
        },

        new User
        {
            UserId = 9,
            Username = "ware02",
            Password = "123456",
            FullName = "Bùi Kiểm Kê",
            Role = "Warehouse"
        },

        new User
        {
            UserId = 10,
            Username = "ware03",
            Password = "123456",
            FullName = "Dương Thủ Kho",
            Role = "Warehouse"
        },

        new User
        {
            UserId = 11,
            Username = "ware04",
            Password = "123456",
            FullName = "Lý Nhập Hàng",
            Role = "Warehouse"
        },

        new User
        {
            UserId = 12,
            Username = "admin_backup",
            Password = "123456",
            FullName = "Đặng Hỗ Trợ",
            Role = "Admin"
        },

        new User
        {
            UserId = 13,
            Username = "cashier06",
            Password = "123456",
            FullName = "Hồ Ca Chiều",
            Role = "Cashier"
        },

        new User
        {
            UserId = 14,
            Username = "ware05",
            Password = "123456",
            FullName = "Trương Vận Chuyển",
            Role = "Warehouse"
        },

        new User
        {
            UserId = 15,
            Username = "supervisor",
            Password = "123456",
            FullName = "Mai Giám Sát",
            Role = "Admin"
        }
    );
        }

    }
}


