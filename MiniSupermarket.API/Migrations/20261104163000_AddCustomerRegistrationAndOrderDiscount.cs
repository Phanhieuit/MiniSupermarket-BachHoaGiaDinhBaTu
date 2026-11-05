using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MiniSupermarket.API.Data;

#nullable disable
namespace MiniSupermarket.API.Migrations;

[DbContext(typeof(SupermarketDbContext))]
[Migration("20261104163000_AddCustomerRegistrationAndOrderDiscount")]
public sealed class AddCustomerRegistrationAndOrderDiscount : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Để NULL với khách cũ: không tự tính họ là khách mới hôm nay.
        migrationBuilder.AddColumn<DateTime>(
            name: "RegisteredAtUtc", table: "Customers",
            type: "datetime2", nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "DiscountAmount", table: "Orders",
            type: "decimal(18,2)", nullable: false, defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "RegisteredAtUtc", table: "Customers");
        migrationBuilder.DropColumn(name: "DiscountAmount", table: "Orders");
    }
}
