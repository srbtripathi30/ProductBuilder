using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Stakeholders;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api/insurers")]
[Authorize]
public class InsurersController : ControllerBase
{
    private readonly AppDbContext _db;
    public InsurersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<InsurerDto>>> GetAll()
        => Ok((await _db.Insurers.ToListAsync()).Select(MapToDto));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InsurerDto>> GetById(Guid id)
    {
        var i = await _db.Insurers.FindAsync(id);
        return i == null ? NotFound() : Ok(MapToDto(i));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InsurerDto>> Create([FromBody] CreateInsurerRequest request)
    {
        if (await _db.Insurers.AnyAsync(i => i.Code == request.Code))
            return Conflict(new { message = "Insurer code already exists" });
        var insurer = new Insurer { Name = request.Name, Code = request.Code.ToUpper(), LicenseNo = request.LicenseNo, Address = request.Address, Phone = request.Phone, Email = request.Email };
        _db.Insurers.Add(insurer);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = insurer.Id }, MapToDto(insurer));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InsurerDto>> Update(Guid id, [FromBody] UpdateInsurerRequest request)
    {
        var insurer = await _db.Insurers.FindAsync(id);
        if (insurer == null) return NotFound();
        insurer.Name = request.Name; insurer.LicenseNo = request.LicenseNo;
        insurer.Address = request.Address; insurer.Phone = request.Phone;
        insurer.Email = request.Email; insurer.IsActive = request.IsActive;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(insurer));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var insurer = await _db.Insurers.FindAsync(id);
        if (insurer == null) return NotFound();
        insurer.IsActive = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static InsurerDto MapToDto(Insurer i) => new()
    {
        Id = i.Id, Name = i.Name, Code = i.Code, LicenseNo = i.LicenseNo,
        Address = i.Address, Phone = i.Phone, Email = i.Email, IsActive = i.IsActive, CreatedAt = i.CreatedAt
    };
}
