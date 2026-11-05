
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using MiniSupermarket.API.DTOs;

namespace MiniSupermarket.API.Controllers
{
    //TC29: chặn API chưa đăng nhập
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _db;
        private readonly IConfiguration _configuration;

        public ProductsController(SupermarketDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // =====================================================
        // BUILD QUERY - LINQ JOIN PRODUCTS VÀ CATEGORIES
        // =====================================================
        private IQueryable<ProductResponse> BuildQuery()
        {
            return from p in _db.Products.AsNoTracking()
                   join c in _db.Categories.AsNoTracking()
                       on p.CategoryId equals c.CategoryId into cg
                   from c in cg.DefaultIfEmpty()
                   select new ProductResponse
                   {
                       ProductId = p.ProductId,
                       Barcode = p.Barcode,
                       ProductName = p.ProductName,
                       Price = p.Price,
                       StockQuantity = p.StockQuantity,
                       CategoryId = p.CategoryId,
                       CategoryName = c != null
                           ? c.CategoryName
                           : string.Empty
                   };
        }

        // =====================================================
        // GET api/products
        // Lấy toàn bộ sản phẩm
        // =====================================================
        // TC26: Eager loading bằng Include(p => p.Category).
        // TC27: AsNoTracking để tối ưu truy vấn chỉ đọc.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .OrderBy(p => p.ProductId)
                .ToListAsync();

            var list = products.Select(p => new ProductResponse
            {
                ProductId = p.ProductId,
                Barcode = p.Barcode,
                ProductName = p.ProductName,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName ?? string.Empty
            }).ToList();

            return Ok(list);
        }

        // =====================================================
        // GET api/products/5
        // Lấy sản phẩm theo ID
        // =====================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await BuildQuery()
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sản phẩm."
                });
            }

            return Ok(product);
        }

        // =====================================================
        // GET api/products/barcode/BT000001
        // Tìm sản phẩm theo mã vạch cho POS
        // =====================================================
        // TC32: ADO.NET (SqlConnection + SqlCommand) tra Barcode.
        // TC33: SqlDataReader và SqlParameter chống SQL Injection.
        // Giữ nguyên URL để WinForms POS không cần sửa.
        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return BadRequest(new { message = "Mã vạch không được để trống." });

            barcode = barcode.Trim();
            if (barcode.Length > 50)
                return BadRequest(new { message = "Mã vạch tối đa 50 ký tự." });

            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Thiếu DefaultConnection.");

            const string sql = @"
                SELECT TOP (1) p.ProductId, p.Barcode, p.ProductName,
                    p.Price, p.StockQuantity, p.CategoryId, c.CategoryName
                FROM dbo.Products AS p
                LEFT JOIN dbo.Categories AS c ON c.CategoryId = p.CategoryId
                WHERE p.Barcode = @Barcode";
            //TC32: ADO
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);

            //TC33
            command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 50).Value = barcode;

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (!await reader.ReadAsync())
                return NotFound(new { message = "Không tìm thấy sản phẩm có mã vạch này." });

            var product = new ProductResponse
            {
                ProductId = reader.GetInt32(0),
                Barcode = reader.GetString(1),
                ProductName = reader.GetString(2),
                Price = reader.GetDecimal(3),
                StockQuantity = reader.GetInt32(4),
                CategoryId = reader.GetInt32(5),
                CategoryName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
            };
            return Ok(product);
        }

        // =====================================================
        // POST api/products
        // Thêm sản phẩm - Admin, Warehouse
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create(
            [FromBody] ProductRequest req)
        {
            string barcode = req.Barcode.Trim();

            // Kiểm tra danh mục tồn tại
            if (!await _db.Categories.AnyAsync(
                c => c.CategoryId == req.CategoryId))
            {
                return BadRequest(new
                {
                    message = "Nhóm hàng không tồn tại."
                });
            }

            // Kiểm tra mã vạch trùng
            if (await _db.Products.AnyAsync(
                p => p.Barcode == barcode))
            {
                return Conflict(new
                {
                    message = "Mã vạch đã tồn tại."
                });
            }

            var entity = new Product
            {
                Barcode = barcode,
                ProductName = req.ProductName.Trim(),
                Price = req.Price,
                StockQuantity = req.StockQuantity,
                CategoryId = req.CategoryId
            };

            _db.Products.Add(entity);
            await _db.SaveChangesAsync();

            var created = await BuildQuery()
                .FirstAsync(p => p.ProductId == entity.ProductId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = entity.ProductId },
                created
            );
        }

        // =====================================================
        // PUT api/products/5
        // Cập nhật sản phẩm - Admin, Warehouse
        // =====================================================
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ProductRequest req)
        {
            var entity = await _db.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (entity == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sản phẩm."
                });
            }

            string barcode = req.Barcode.Trim();

            // Kiểm tra danh mục
            if (!await _db.Categories.AnyAsync(
                c => c.CategoryId == req.CategoryId))
            {
                return BadRequest(new
                {
                    message = "Nhóm hàng không tồn tại."
                });
            }

            // Kiểm tra mã vạch trùng với sản phẩm khác
            if (await _db.Products.AnyAsync(
                p => p.Barcode == barcode && p.ProductId != id))
            {
                return Conflict(new
                {
                    message = "Mã vạch đã được dùng cho sản phẩm khác."
                });
            }

            entity.Barcode = barcode;
            entity.ProductName = req.ProductName.Trim();
            entity.Price = req.Price;
            entity.StockQuantity = req.StockQuantity;
            entity.CategoryId = req.CategoryId;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================
        // DELETE api/products/5
        // Xóa sản phẩm - Admin, Warehouse
        // =====================================================
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (entity == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sản phẩm."
                });
            }

            try
            {
                _db.Products.Remove(entity);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Không thể xóa: sản phẩm đã phát sinh giao dịch."
                });
            }
        }

        // =====================================================
        // GET api/products/search?keyword=Gạo
        // BƯỚC 24.2 - TÌM KIẾM SẢN PHẨM BẰNG LINQ
        // =====================================================
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? keyword)
        {
            // Kiểm tra từ khóa rỗng
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập từ khóa tìm kiếm."
                });
            }

            keyword = keyword.Trim();

            // Tìm theo tên sản phẩm hoặc mã vạch
            var products = await BuildQuery()
                .Where(p =>
                    p.ProductName.Contains(keyword) ||
                    p.Barcode.Contains(keyword))
                .OrderBy(p => p.ProductId)
                .ToListAsync();

            return Ok(products);
        }

        // =====================================================
        // GET api/products/filter
        // BƯỚC 24.3 - LỌC SẢN PHẨM BẰNG LINQ
        // =====================================================
        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
            {
                return BadRequest(new
                {
                    message = "Giá lọc không được âm."
                });
            }

            if (minPrice.HasValue && maxPrice.HasValue
                && minPrice > maxPrice)
            {
                return BadRequest(new
                {
                    message = "Giá tối thiểu không được lớn hơn giá tối đa."
                });
            }

            var query = BuildQuery();

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == categoryId.Value);
            }

            // Lọc theo giá thấp nhất
            if (minPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= minPrice.Value);
            }

            // Lọc theo giá cao nhất
            if (maxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= maxPrice.Value);
            }

            var products = await query
                .OrderBy(p => p.ProductId)
                .ToListAsync();

            return Ok(products);
        }

        // =====================================================
        // GET api/products/paged
        // BƯỚC 25 - PHÂN TRANG SẢN PHẨM BẰNG LINQ
        // =====================================================
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Kiểm tra tham số phân trang
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new
                {
                    message = "Số trang phải >= 1 và kích thước trang từ 1 đến 100."
                });
            }

            // Đếm tổng số sản phẩm
            var totalItems = await BuildQuery().CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize);

            // Tránh tràn số khi tính vị trí bắt đầu
            long offset = ((long)pageNumber - 1) * pageSize;

            if (offset > int.MaxValue)
            {
                return BadRequest(new
                {
                    message = "Số trang vượt quá giới hạn cho phép."
                });
            }

            // LINQ phân trang bằng Skip và Take
            var products = await BuildQuery()
                .OrderBy(p => p.ProductId)
                .Skip((int)offset)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Data = products
            });
        }


    }
}
