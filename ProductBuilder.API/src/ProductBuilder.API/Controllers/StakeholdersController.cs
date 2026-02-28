using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Stakeholders;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Authorize]
public class StakeholdersController : ControllerBase
{
    private readonly AppDbContext _db;
    public StakeholdersController(AppDbContext db) => _db = db;

    // --- Underwriters ---
    [HttpGet("api/underwriters")]
    public async Task<ActionResult<List<UnderwriterDto>>> GetUnderwriters()
    {
        var list = await _db.Underwriters.Include(u => u.User).ToListAsync();
        return Ok(list.Select(u => new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt }));
    }

    [HttpGet("api/underwriters/{id:guid}")]
    public async Task<ActionResult<UnderwriterDto>> GetUnderwriter(Guid id)
    {
        var u = await _db.Underwriters.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == id);
        if (u == null) return NotFound();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    [HttpPost("api/underwriters")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnderwriterDto>> CreateUnderwriter([FromBody] CreateUnderwriterRequest req)
    {
        var u = new Underwriter { UserId = req.UserId, LicenseNo = req.LicenseNo, Specialization = req.Specialization, AuthorityLimit = req.AuthorityLimit };
        _db.Underwriters.Add(u); await _db.SaveChangesAsync();
        await _db.Entry(u).Reference(x => x.User).LoadAsync();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    [HttpPut("api/underwriters/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UnderwriterDto>> UpdateUnderwriter(Guid id, [FromBody] UpdateUnderwriterRequest req)
    {
        var u = await _db.Underwriters.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound();
        u.LicenseNo = req.LicenseNo; u.Specialization = req.Specialization; u.AuthorityLimit = req.AuthorityLimit;
        await _db.SaveChangesAsync();
        return Ok(new UnderwriterDto { Id = u.Id, UserId = u.UserId, UserName = $"{u.User.FirstName} {u.User.LastName}", UserEmail = u.User.Email, LicenseNo = u.LicenseNo, Specialization = u.Specialization, AuthorityLimit = u.AuthorityLimit, CreatedAt = u.CreatedAt });
    }

    // --- Brokers ---
    [HttpGet("api/brokers")]
    public async Task<ActionResult<List<BrokerDto>>> GetBrokers()
    {
        var list = await _db.Brokers.Include(b => b.User).Include(b => b.Insurer).ToListAsync();
        return Ok(list.Select(b => new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt }));
    }

    [HttpGet("api/brokers/{id:guid}")]
    public async Task<ActionResult<BrokerDto>> GetBroker(Guid id)
    {
        var b = await _db.Brokers.Include(x => x.User).Include(x => x.Insurer).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        return Ok(new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, InsurerName = b.Insurer?.Name, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt });
    }

    [HttpPost("api/brokers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrokerDto>> CreateBroker([FromBody] CreateBrokerRequest req)
    {
        var b = new Broker { UserId = req.UserId, InsurerId = req.InsurerId, CompanyName = req.CompanyName, LicenseNo = req.LicenseNo, CommissionRate = req.CommissionRate };
        _db.Brokers.Add(b); await _db.SaveChangesAsync();
        await _db.Entry(b).Reference(x => x.User).LoadAsync();
        return Ok(new BrokerDto { Id = b.Id, UserId = b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", UserEmail = b.User.Email, InsurerId = b.InsurerId, CompanyName = b.CompanyName, LicenseNo = b.LicenseNo, CommissionRate = b.CommissionRate, IsActive = b.IsActive, CreatedAt = b.CreatedAt });
    }

    [HttpPut("api/brokers/{id:guid}")]
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
}
