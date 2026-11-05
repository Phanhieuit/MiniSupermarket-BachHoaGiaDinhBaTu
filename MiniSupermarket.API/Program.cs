using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// SQL Server (LocalDB mặc định, thay chuỗi kết nối trong appsettings.json nếu dùng SQL Express).
builder.Services.AddDbContext<SupermarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình JWT
var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? "BachHoaGiaDinhBaTuRestfulWebApi2026SecretKeyChangeBeforeProduction!!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret)
        ),

        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

// Authorization
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Cấu hình CORS cho Web API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger + JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token theo dạng: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Tạo CSDL demo sạch với dữ liệu hàng bách hóa; không áp dụng trên CSDL cũ.
// Khi triển khai thực tế nên dùng EF Core migrations thay cho EnsureCreated.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupermarketDbContext>();

    if (!db.Database.CanConnect())
    {
        throw new InvalidOperationException(
            "Khong the ket noi SQL Server.");
    }

    if (db.Database.GetPendingMigrations().Any())
    {
        throw new InvalidOperationException(
            "Database chua cap nhat Migrations. " +
            "Hay chay Update-Database tren dung database.");
    }

    app.Logger.LogInformation(
    "Database dang su dung: {Database}",
    db.Database.GetDbConnection().Database
);

    // Tạo một tài khoản quản trị chỉ dành cho môi trường phát triển.
    if (app.Environment.IsDevelopment())
    {
        var demoPassword = app.Configuration["BootstrapAdmin:Password"];
        if (!string.IsNullOrWhiteSpace(demoPassword) && !db.Users.Any(u => u.Username == "admin"))
        {
            var admin = new User
            {
                Username = "admin", FullName = "Quản trị Bách hóa Gia đình Bà Tư",
                Role = "Admin", IsActive = true
            };
            admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, demoPassword);
            db.Users.Add(admin);
            db.SaveChanges();
        }
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowClient");

// Authentication phải đứng trước Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
