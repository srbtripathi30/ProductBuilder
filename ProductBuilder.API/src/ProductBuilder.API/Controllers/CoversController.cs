using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Products;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Authorize]
public class CoversController : ControllerBase
{
    private readonly AppDbContext _db;
    public CoversController(AppDbContext db) => _db = db;

    [HttpGet("api/coverages/{coverageId:guid}/covers")]
    public async Task<ActionResult<List<CoverDto>>> GetAll(Guid coverageId)
    {
        var covers = await _db.Covers.Where(c => c.CoverageId == coverageId).OrderBy(c => c.SequenceNo).ToListAsync();
        return Ok(covers.Select(MapToDto));
    }

    [HttpPost("api/coverages/{coverageId:guid}/covers")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<CoverDto>> Create(Guid coverageId, [FromBody] CreateCoverRequest request)
    {
        if (!await _db.Coverages.AnyAsync(c => c.Id == coverageId)) return NotFound();
        var cover = new Cover
        {
            CoverageId = coverageId, Name = request.Name, Code = request.Code.ToUpper(),
            Description = request.Description, IsMandatory = request.IsMandatory, SequenceNo = request.SequenceNo
        };
        _db.Covers.Add(cover);
        await _db.SaveChangesAsync();
        return Ok(MapToDto(cover));
    }

    [HttpPut("api/covers/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<CoverDto>> Update(Guid id, [FromBody] UpdateCoverRequest request)
    {
        var cover = await _db.Covers.FindAsync(id);
        if (cover == null) return NotFound();
        cover.Name = request.Name; cover.Description = request.Description;
        cover.IsMandatory = request.IsMandatory; cover.SequenceNo = request.SequenceNo;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(cover));
    }

    [HttpDelete("api/covers/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var cover = await _db.Covers.FindAsync(id);
        if (cover == null) return NotFound();
        _db.Covers.Remove(cover);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static CoverDto MapToDto(Cover c) => new()
    {
        Id = c.Id, CoverageId = c.CoverageId, Name = c.Name, Code = c.Code,
        Description = c.Description, IsMandatory = c.IsMandatory, SequenceNo = c.SequenceNo, CreatedAt = c.CreatedAt
    };
}
