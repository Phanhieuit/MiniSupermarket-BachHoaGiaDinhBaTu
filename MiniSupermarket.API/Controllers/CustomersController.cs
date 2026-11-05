
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.DTOs;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Admin,Cashier")]
public class CustomersController(SupermarketDbContext db)
    : ControllerBase
{
    private IQueryable<CustomerResponse> CustomerQuery()
    {
        return db.Customers.AsNoTracking()
            .Select(c => new CustomerResponse
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                MembershipRank = c.MembershipRank,
                RewardPoints = c.RewardPoints
            });
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var customers = await CustomerQuery()
            .OrderBy(c => c.CustomerName)
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("phone/{phone}")]
    public async Task<IActionResult> ByPhone(string phone)
    {
        var customer = await CustomerQuery()
            .FirstOrDefaultAsync(c => c.PhoneNumber == phone);

        if (customer == null)
            return NotFound(new
            {
                message = "Không tìm thấy khách hàng."
            });

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [FromBody] CustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName) ||
            string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return BadRequest(new
            {
                message = "Cần nhập họ tên và số điện thoại."
            });
        }

        string phone = request.PhoneNumber.Trim();

        bool exists = await db.Customers
            .AnyAsync(c => c.PhoneNumber == phone);

        if (exists)
            return BadRequest(new
            {
                message = "Số điện thoại đã tồn tại."
            });

        var customer = new Customer
        {
            CustomerName = request.CustomerName.Trim(),
            PhoneNumber = phone,
            Address = request.Address?.Trim(),
            RegisteredAtUtc = DateTime.UtcNow
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var response = new CustomerResponse
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            MembershipRank = customer.MembershipRank,
            RewardPoints = customer.RewardPoints
        };

        return Ok(response);
    }
}
