using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Lob;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api/lob")]
[Authorize]
public class LinesOfBusinessController : ControllerBase
{
    private readonly AppDbContext _db;

    public LinesOfBusinessController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<LobDto>>> GetAll()
    {
        var lobs = await _db.LinesOfBusiness.ToListAsync();
        return Ok(lobs.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LobDto>> GetById(Guid id)
    {
        var lob = await _db.LinesOfBusiness.FindAsync(id);
        if (lob == null) return NotFound();
        return Ok(MapToDto(lob));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<LobDto>> Create([FromBody] CreateLobRequest request)
    {
        if (await _db.LinesOfBusiness.AnyAsync(l => l.Code == request.Code))
            return Conflict(new { message = "LOB code already exists" });
        var lob = new LineOfBusiness { Name = request.Name, Code = request.Code.ToUpper(), Description = request.Description };
        _db.LinesOfBusiness.Add(lob);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = lob.Id }, MapToDto(lob));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<LobDto>> Update(Guid id, [FromBody] UpdateLobRequest request)
    {
        var lob = await _db.LinesOfBusiness.FindAsync(id);
        if (lob == null) return NotFound();
        lob.Name = request.Name;
        lob.Description = request.Description;
        lob.IsActive = request.IsActive;
        lob.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(lob));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var lob = await _db.LinesOfBusiness.FindAsync(id);
        if (lob == null) return NotFound();
        lob.IsActive = false;
        lob.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static LobDto MapToDto(LineOfBusiness l) => new()
    {
        Id = l.Id, Name = l.Name, Code = l.Code, Description = l.Description,
        IsActive = l.IsActive, CreatedAt = l.CreatedAt
    };
}
