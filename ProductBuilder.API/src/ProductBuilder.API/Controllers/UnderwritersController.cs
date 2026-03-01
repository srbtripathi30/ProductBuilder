using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Stakeholders;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api/underwriters")]
[Authorize]
public class UnderwritersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UnderwritersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<UnderwriterDto>>> GetAll()
    {
        var list = await _db.Underwriters.Include(u => u.User).ToListAsync();
        return Ok(list.Select(u => new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt }));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UnderwriterDto>> GetById(Guid id)
    {
        var u = await _db.Underwriters.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == id);
        if (u == null) return NotFound();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnderwriterDto>> Create([FromBody] CreateUnderwriterRequest req)
    {
        var user = await _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == req.UserId);
        if (user == null) return BadRequest(new { message = "User not found" });
        if (!user.IsActive) return BadRequest(new { message = "User is inactive" });
        if (!string.Equals(user.Role.Name, "Underwriter", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Selected user is not an Underwriter" });
        if (await _db.Underwriters.AnyAsync(x => x.UserId == req.UserId))
            return Conflict(new { message = "This user is already assigned as an underwriter" });

        var u = new Underwriter { UserId = req.UserId, LicenseNo = req.LicenseNo, Specialization = req.Specialization, AuthorityLimit = req.AuthorityLimit };
        _db.Underwriters.Add(u); await _db.SaveChangesAsync();
        await _db.Entry(u).Reference(x => x.User).LoadAsync();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnderwriterDto>> Update(Guid id, [FromBody] UpdateUnderwriterRequest req)
    {
        var u = await _db.Underwriters.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound();
        u.LicenseNo = req.LicenseNo; u.Specialization = req.Specialization; u.AuthorityLimit = req.AuthorityLimit;
        await _db.SaveChangesAsync();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var u = await _db.Underwriters.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound();

        try
        {
            var linkedQuotes = await _db.Quotes.Where(q => q.UnderwriterId == id).ToListAsync();
            foreach (var quote in linkedQuotes)
            {
                quote.UnderwriterId = null;
                quote.UpdatedAt = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync();

            _db.Underwriters.Remove(u);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = $"Failed to delete underwriter: {ex.InnerException?.Message ?? ex.Message}" });
        }
    }
}
