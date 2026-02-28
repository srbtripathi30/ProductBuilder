using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Products;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Authorize]
public class CoveragesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CoveragesController(AppDbContext db) => _db = db;

    [HttpGet("api/products/{productId:guid}/coverages")]
    public async Task<ActionResult<List<CoverageDto>>> GetAll(Guid productId)
    {
        var coverages = await _db.Coverages.Include(c => c.Covers)
            .Where(c => c.ProductId == productId).OrderBy(c => c.SequenceNo).ToListAsync();
        return Ok(coverages.Select(MapToDto));
    }

    [HttpPost("api/products/{productId:guid}/coverages")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<CoverageDto>> Create(Guid productId, [FromBody] CreateCoverageRequest request)
    {
        if (!await _db.Products.AnyAsync(p => p.Id == productId)) return NotFound();
        var coverage = new Coverage
        {
            ProductId = productId, Name = request.Name, Code = request.Code.ToUpper(),
            Description = request.Description, IsMandatory = request.IsMandatory, SequenceNo = request.SequenceNo
        };
        _db.Coverages.Add(coverage);
        await _db.SaveChangesAsync();
        coverage.Covers = new List<Cover>();
        return Ok(MapToDto(coverage));
    }

    [HttpPut("api/coverages/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<CoverageDto>> Update(Guid id, [FromBody] UpdateCoverageRequest request)
    {
        var coverage = await _db.Coverages.Include(c => c.Covers).FirstOrDefaultAsync(c => c.Id == id);
        if (coverage == null) return NotFound();
        coverage.Name = request.Name; coverage.Description = request.Description;
        coverage.IsMandatory = request.IsMandatory; coverage.SequenceNo = request.SequenceNo;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(coverage));
    }

    [HttpDelete("api/coverages/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var coverage = await _db.Coverages.FindAsync(id);
        if (coverage == null) return NotFound();
        _db.Coverages.Remove(coverage);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static CoverageDto MapToDto(Coverage c) => new()
    {
        Id = c.Id, ProductId = c.ProductId, Name = c.Name, Code = c.Code,
        Description = c.Description, IsMandatory = c.IsMandatory, SequenceNo = c.SequenceNo,
        CreatedAt = c.CreatedAt, Covers = c.Covers.Select(cv => new CoverDto
        {
            Id = cv.Id, CoverageId = cv.CoverageId, Name = cv.Name, Code = cv.Code,
            Description = cv.Description, IsMandatory = cv.IsMandatory, SequenceNo = cv.SequenceNo, CreatedAt = cv.CreatedAt
        }).ToList()
    };
}
