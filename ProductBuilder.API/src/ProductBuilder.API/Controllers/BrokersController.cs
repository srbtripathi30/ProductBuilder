using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Stakeholders;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api/brokers")]
[Authorize]
public class BrokersController : ControllerBase
{
    private readonly AppDbContext _db;
    public BrokersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<BrokerDto>>> GetAll()
    {
        var list = await _db.Brokers.Include(b => b.User).Include(b => b.Insurer).ToListAsync();
        return Ok(list.Select(b => new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt }));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BrokerDto>> GetById(Guid id)
    {
        var b = await _db.Brokers.Include(x => x.User).Include(x => x.Insurer).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        return Ok(new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrokerDto>> Create([FromBody] CreateBrokerRequest req)
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrokerDto>> Update(Guid id, [FromBody] UpdateBrokerRequest req)
    {
        var b = await _db.Brokers.Include(x => x.User).Include(x => x.Insurer).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        b.InsurerId = req.InsurerId; b.CompanyName = req.CompanyName; b.LicenseNo = req.LicenseNo;
        b.CommissionRate = req.CommissionRate; b.IsActive = req.IsActive;
        await _db.SaveChangesAsync();
        return Ok(new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
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
