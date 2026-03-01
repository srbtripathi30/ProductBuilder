using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Stakeholders;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class StakeholdersController : ControllerBase
{
    private readonly AppDbContext _db;
    public StakeholdersController(AppDbContext db) => _db = db;

    // --- Underwriters ---
    [HttpGet("underwriters")]
    public async Task<ActionResult<List<UnderwriterDto>>> GetUnderwriters()
    {
        var list = await _db.Underwriters.Include(u => u.User).ToListAsync();
        return Ok(list.Select(u => new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt }));
    }

    [HttpGet("underwriters/{id:guid}")]
    public async Task<ActionResult<UnderwriterDto>> GetUnderwriter(Guid id)
    {
        var u = await _db.Underwriters.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == id);
        if (u == null) return NotFound();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    [HttpPost("underwriters")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnderwriterDto>> CreateUnderwriter([FromBody] CreateUnderwriterRequest req)
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

    [HttpPut("underwriters/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnderwriterDto>> UpdateUnderwriter(Guid id, [FromBody] UpdateUnderwriterRequest req)
    {
        var u = await _db.Underwriters.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound();
        u.LicenseNo = req.LicenseNo; u.Specialization = req.Specialization; u.AuthorityLimit = req.AuthorityLimit;
        await _db.SaveChangesAsync();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    [HttpDelete("underwriters/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUnderwriter(Guid id)
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

            // Persist FK nulling first to avoid DB constraint ordering issues.
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

    // --- Brokers ---
    [HttpGet("brokers")]
    public async Task<ActionResult<List<BrokerDto>>> GetBrokers()
    {
        var list = await _db.Brokers.Include(b => b.User).Include(b => b.Insurer).ToListAsync();
        return Ok(list.Select(b => new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt }));
    }

    [HttpGet("brokers/{id:guid}")]
    public async Task<ActionResult<BrokerDto>> GetBroker(Guid id)
    {
        var b = await _db.Brokers.Include(x => x.User).Include(x => x.Insurer).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        return Ok(new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt });
    }

    [HttpPost("brokers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrokerDto>> CreateBroker([FromBody] CreateBrokerRequest req)
    {
        var user = await _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == req.UserId);
        if (user == null) return BadRequest(new { message = "User not found" });
        if (!user.IsActive) return BadRequest(new { message = "User is inactive" });
        if (!string.Equals(user.Role.Name, "Broker", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Selected user is not a Broker" });
        if (await _db.Brokers.AnyAsync(x => x.UserId == req.UserId))
            return Conflict(new { message = "This user is already assigned as a broker" });

        var b = new Broker { UserId = req.UserId, InsurerId = req.InsurerId, CompanyName = req.CompanyName, LicenseNo = req.LicenseNo, CommissionRate = req.CommissionRate };
        _db.Brokers.Add(b); await _db.SaveChangesAsync();
        await _db.Entry(b).Reference(x => x.User).LoadAsync();
        return Ok(new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt });
    }

    [HttpPut("brokers/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrokerDto>> UpdateBroker(Guid id, [FromBody] UpdateBrokerRequest req)
    {
        var b = await _db.Brokers.Include(x => x.User).Include(x => x.Insurer).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        b.InsurerId = req.InsurerId; b.CompanyName = req.CompanyName; b.LicenseNo = req.LicenseNo;
        b.CommissionRate = req.CommissionRate; b.IsActive = req.IsActive;
        await _db.SaveChangesAsync();
        return Ok(new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt });
    }

    [HttpDelete("brokers/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBroker(Guid id)
    {
        var b = await _db.Brokers.FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();

        try
        {
            var linkedQuotes = await _db.Quotes.Where(q => q.BrokerId == id).ToListAsync();
            foreach (var quote in linkedQuotes)
            {
                quote.BrokerId = null;
                quote.UpdatedAt = DateTime.UtcNow;
            }

            // Persist FK nulling first to avoid DB constraint ordering issues.
            await _db.SaveChangesAsync();

            _db.Brokers.Remove(b);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = $"Failed to delete broker: {ex.InnerException?.Message ?? ex.Message}" });
        }
    }
}
