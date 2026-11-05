using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using MiniSupermarket.API.DTOs;

namespace MiniSupermarket.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController(SupermarketDbContext db)
    : ControllerBase
{
    private IQueryable<CategoryResponse> Rows() =>
        db.Categories
            .AsNoTracking()
            .OrderBy(c => c.CategoryId)
            .Select(c => new CategoryResponse(
                c.CategoryId,
                c.CategoryName,
                c.Description
            ));

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await Rows().ToListAsync());
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await db.Categories
            .AsNoTracking()
            .Where(c => c.CategoryId == id)
            .Select(c => new CategoryResponse(
                c.CategoryId,
                c.CategoryName,
                c.Description
            ))
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy danh mục."
            });
        }

        return Ok(category);
    }



    [HttpGet("search")]
    public async Task<IActionResult> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest(new
            {
                message = "Vui lòng nhập từ khóa."
            });
        }

        keyword = keyword.Trim();

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => c.CategoryName.Contains(keyword))
            .OrderBy(c => c.CategoryId)
            .Select(c => new CategoryResponse(
                c.CategoryId,
                c.CategoryName,
                c.Description
            ))
            .ToListAsync();

        return Ok(categories);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Warehouse")]
    public async Task<IActionResult> Create(CategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName))
            return BadRequest(new
            {
                message = "Tên không được trống."
            });

        var category = new Category
        {
            CategoryName = request.CategoryName.Trim(),
            Description = request.Description
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.CategoryId },
            new CategoryResponse(
                category.CategoryId,
                category.CategoryName,
                category.Description
            ));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Warehouse")]
    public async Task<IActionResult> Update(
        int id, CategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName))
            return BadRequest(new
            {
                message = "Tên không được trống."
            });

        var category = await db.Categories.FindAsync(id);

        if (category == null)
            return NotFound();

        category.CategoryName = request.CategoryName.Trim();
        category.Description = request.Description;

        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Warehouse")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories.FindAsync(id);

        if (category == null)
            return NotFound();

        if (await db.Products.AnyAsync(p => p.CategoryId == id))
            return BadRequest(new
            {
                message = "Nhóm hàng đang có sản phẩm, không thể xóa."
            });

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        return NoContent();
    }
}