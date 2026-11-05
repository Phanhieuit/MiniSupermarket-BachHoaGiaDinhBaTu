using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;
namespace MiniSupermarket.API.Data;

public static class PracticeDatabase
{
    public static void Initialize(SupermarketDbContext db, bool development)
    {
        // Bổ sung bảng bài 4 vào database demo cũ, không xóa dữ liệu sẵn có.
        using var stream = typeof(PracticeDatabase).Assembly.GetManifestResourceStream("MiniSupermarket.API.Database.Orders.sql")
            ?? throw new InvalidOperationException("Không tìm thấy script Orders.sql.");
        using var reader = new StreamReader(stream);
        db.Database.ExecuteSqlRaw(reader.ReadToEnd());
        if (!development) return;
        // Chỉ chuyển đổi dữ liệu mẫu chưa có hash; không mở khóa lại tài khoản đã quản trị.
        var hasher = new PasswordHasher<User>();
        foreach (var user in db.Users.Where(u => u.PasswordHash == "" && u.Password != ""))
        {
            user.PasswordHash = hasher.HashPassword(user, user.Password);
            user.Password = string.Empty;
            user.IsActive = true;
        }
        db.SaveChanges();
    }
}

