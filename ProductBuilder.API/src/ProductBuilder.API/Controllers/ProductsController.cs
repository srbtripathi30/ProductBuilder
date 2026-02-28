using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Products;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;
using System.Security.Claims;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] string? status, [FromQuery] Guid? lobId)
    {
        var query = _db.Products.Include(p => p.Lob).Include(p => p.Insurer).AsQueryable();
        if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.Status == status);
        if (lobId.HasValue) query = query.Where(p => p.LobId == lobId.Value);
        return Ok((await query.ToListAsync()).Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var p = await _db.Products.Include(p => p.Lob).Include(p => p.Insurer)
            .Include(p => p.Coverages).ThenInclude(c => c.Covers)
            .FirstOrDefaultAsync(p => p.Id == id);
        return p == null ? NotFound() : Ok(MapToDto(p));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
    {
        if (await _db.Products.AnyAsync(p => p.Code == request.Code))
            return Conflict(new { message = "Product code already exists" });
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var product = new Product
        {
            LobId = request.LobId, InsurerId = request.InsurerId, Name = request.Name,
            Code = request.Code.ToUpper(), Description = request.Description, Version = request.Version,
            EffectiveDate = request.EffectiveDate, ExpiryDate = request.ExpiryDate, CreatedBy = userId
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        await _db.Entry(product).Reference(p => p.Lob).LoadAsync();
        await _db.Entry(product).Reference(p => p.Insurer).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapToDto(product));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await _db.Products.Include(p => p.Lob).Include(p => p.Insurer).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        if (product.Status == "Active") return BadRequest(new { message = "Cannot edit an active product. Deactivate first." });
        product.Name = request.Name; product.Description = request.Description;
        product.Version = request.Version; product.EffectiveDate = request.EffectiveDate;
        product.ExpiryDate = request.ExpiryDate; product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(product));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<ProductDto>> UpdateStatus(Guid id, [FromBody] UpdateProductStatusRequest request)
    {
        var product = await _db.Products.Include(p => p.Lob).Include(p => p.Insurer).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        var validStatuses = new[] { "Draft", "Active", "Inactive", "Archived" };
        if (!validStatuses.Contains(request.Status))
            return BadRequest(new { message = "Invalid status" });
        product.Status = request.Status; product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(product));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        product.Status = "Archived"; product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id, LobId = p.LobId, LobName = p.Lob?.Name ?? string.Empty,
        InsurerId = p.InsurerId, InsurerName = p.Insurer?.Name ?? string.Empty,
        Name = p.Name, Code = p.Code, Description = p.Description, Version = p.Version,
        Status = p.Status, EffectiveDate = p.EffectiveDate, ExpiryDate = p.ExpiryDate,
        CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt
    };
}
