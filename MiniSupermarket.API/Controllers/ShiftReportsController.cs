using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MiniSupermarket.API.Controllers;

// TC34: gọi SQL Server Stored Procedure bằng CommandType.StoredProcedure.
[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ShiftReportsController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("shift")]
    public async Task<IActionResult> RevenueByShift(
        [FromQuery] DateTime? date,
        [FromQuery] int shift)
    {
        if (!date.HasValue || date.Value == default || shift is < 1 or > 3)
            return BadRequest(new { message = "Ngày phải hợp lệ và ca từ 1 đến 3." });

        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Thiếu DefaultConnection.");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.usp_RevenueByShift", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@ReportDate", SqlDbType.Date).Value = date.Value.Date;
        command.Parameters.Add("@Shift", SqlDbType.TinyInt).Value = (byte)shift;

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        if (!await reader.ReadAsync())
            return StatusCode(500, new { message = "Stored Procedure không trả dữ liệu." });

        return Ok(new
        {
            reportDate = date.Value.ToString("yyyy-MM-dd"),
            shift,
            totalOrders = reader.GetInt32(0),
            totalRevenue = reader.GetDecimal(1)
        });
    }
}
