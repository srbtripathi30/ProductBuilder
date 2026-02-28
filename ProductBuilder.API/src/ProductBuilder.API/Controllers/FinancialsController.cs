using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Financials;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Authorize]
public class FinancialsController : ControllerBase
{
    private readonly AppDbContext _db;
    public FinancialsController(AppDbContext db) => _db = db;

    // --- Limits ---
    [HttpGet("api/covers/{coverId:guid}/limits")]
    public async Task<ActionResult<List<LimitDto>>> GetLimits(Guid coverId)
        => Ok((await _db.Limits.Where(l => l.CoverId == coverId).ToListAsync()).Select(MapLimit));

    [HttpPost("api/covers/{coverId:guid}/limits")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<LimitDto>> CreateLimit(Guid coverId, [FromBody] CreateLimitRequest req)
    {
        if (!await _db.Covers.AnyAsync(c => c.Id == coverId)) return NotFound();
        var limit = new Limit { CoverId = coverId, LimitType = req.LimitType, MinAmount = req.MinAmount, MaxAmount = req.MaxAmount, DefaultAmount = req.DefaultAmount, Currency = req.Currency };
        _db.Limits.Add(limit); await _db.SaveChangesAsync();
        return Ok(MapLimit(limit));
    }

    [HttpPut("api/limits/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<LimitDto>> UpdateLimit(Guid id, [FromBody] UpdateLimitRequest req)
    {
        var limit = await _db.Limits.FindAsync(id);
        if (limit == null) return NotFound();
        limit.LimitType = req.LimitType; limit.MinAmount = req.MinAmount; limit.MaxAmount = req.MaxAmount;
        limit.DefaultAmount = req.DefaultAmount; limit.Currency = req.Currency; limit.IsActive = req.IsActive;
        await _db.SaveChangesAsync(); return Ok(MapLimit(limit));
    }

    [HttpDelete("api/limits/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<IActionResult> DeleteLimit(Guid id)
    {
        var limit = await _db.Limits.FindAsync(id);
        if (limit == null) return NotFound();
        _db.Limits.Remove(limit); await _db.SaveChangesAsync(); return NoContent();
    }

    // --- Deductibles ---
    [HttpGet("api/covers/{coverId:guid}/deductibles")]
    public async Task<ActionResult<List<DeductibleDto>>> GetDeductibles(Guid coverId)
        => Ok((await _db.Deductibles.Where(d => d.CoverId == coverId).ToListAsync()).Select(MapDeductible));

    [HttpPost("api/covers/{coverId:guid}/deductibles")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<DeductibleDto>> CreateDeductible(Guid coverId, [FromBody] CreateDeductibleRequest req)
    {
        if (!await _db.Covers.AnyAsync(c => c.Id == coverId)) return NotFound();
        var d = new Deductible { CoverId = coverId, DeductibleType = req.DeductibleType, MinAmount = req.MinAmount, MaxAmount = req.MaxAmount, DefaultAmount = req.DefaultAmount, Currency = req.Currency };
        _db.Deductibles.Add(d); await _db.SaveChangesAsync(); return Ok(MapDeductible(d));
    }

    [HttpPut("api/deductibles/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<DeductibleDto>> UpdateDeductible(Guid id, [FromBody] UpdateDeductibleRequest req)
    {
        var d = await _db.Deductibles.FindAsync(id);
        if (d == null) return NotFound();
        d.DeductibleType = req.DeductibleType; d.MinAmount = req.MinAmount; d.MaxAmount = req.MaxAmount;
        d.DefaultAmount = req.DefaultAmount; d.Currency = req.Currency; d.IsActive = req.IsActive;
        await _db.SaveChangesAsync(); return Ok(MapDeductible(d));
    }

    [HttpDelete("api/deductibles/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<IActionResult> DeleteDeductible(Guid id)
    {
        var d = await _db.Deductibles.FindAsync(id);
        if (d == null) return NotFound();
        _db.Deductibles.Remove(d); await _db.SaveChangesAsync(); return NoContent();
    }

    // --- Premiums ---
    [HttpGet("api/covers/{coverId:guid}/premiums")]
    public async Task<ActionResult<List<PremiumDto>>> GetPremiums(Guid coverId)
        => Ok((await _db.Premiums.Where(p => p.CoverId == coverId).ToListAsync()).Select(MapPremium));

    [HttpPost("api/covers/{coverId:guid}/premiums")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<PremiumDto>> CreatePremium(Guid coverId, [FromBody] CreatePremiumRequest req)
    {
        if (!await _db.Covers.AnyAsync(c => c.Id == coverId)) return NotFound();
        var p = new Premium { CoverId = coverId, PremiumType = req.PremiumType, BaseRate = req.BaseRate, FlatAmount = req.FlatAmount, MinPremium = req.MinPremium, CalculationBasis = req.CalculationBasis, Currency = req.Currency };
        _db.Premiums.Add(p); await _db.SaveChangesAsync(); return Ok(MapPremium(p));
    }

    [HttpPut("api/premiums/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<PremiumDto>> UpdatePremium(Guid id, [FromBody] UpdatePremiumRequest req)
    {
        var p = await _db.Premiums.FindAsync(id);
        if (p == null) return NotFound();
        p.PremiumType = req.PremiumType; p.BaseRate = req.BaseRate; p.FlatAmount = req.FlatAmount;
        p.MinPremium = req.MinPremium; p.CalculationBasis = req.CalculationBasis; p.Currency = req.Currency; p.IsActive = req.IsActive;
        await _db.SaveChangesAsync(); return Ok(MapPremium(p));
    }

    [HttpDelete("api/premiums/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<IActionResult> DeletePremium(Guid id)
    {
        var p = await _db.Premiums.FindAsync(id);
        if (p == null) return NotFound();
        _db.Premiums.Remove(p); await _db.SaveChangesAsync(); return NoContent();
    }

    // --- Modifiers ---
    [HttpGet("api/modifiers")]
    public async Task<ActionResult<List<ModifierDto>>> GetModifiers([FromQuery] Guid? coverId, [FromQuery] Guid? productId)
    {
        var query = _db.Modifiers.AsQueryable();
        if (coverId.HasValue) query = query.Where(m => m.CoverId == coverId.Value);
        if (productId.HasValue) query = query.Where(m => m.ProductId == productId.Value);
        return Ok((await query.ToListAsync()).Select(MapModifier));
    }

    [HttpPost("api/modifiers")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<ModifierDto>> CreateModifier([FromBody] CreateModifierRequest req)
    {
        var m = new Modifier { CoverId = req.CoverId, ProductId = req.ProductId, Name = req.Name, Code = req.Code.ToUpper(), ModifierType = req.ModifierType, ValueType = req.ValueType, MinValue = req.MinValue, MaxValue = req.MaxValue, DefaultValue = req.DefaultValue, IsMandatory = req.IsMandatory, Description = req.Description };
        _db.Modifiers.Add(m); await _db.SaveChangesAsync(); return Ok(MapModifier(m));
    }

    [HttpPut("api/modifiers/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<ActionResult<ModifierDto>> UpdateModifier(Guid id, [FromBody] UpdateModifierRequest req)
    {
        var m = await _db.Modifiers.FindAsync(id);
        if (m == null) return NotFound();
        m.Name = req.Name; m.ModifierType = req.ModifierType; m.ValueType = req.ValueType;
        m.MinValue = req.MinValue; m.MaxValue = req.MaxValue; m.DefaultValue = req.DefaultValue;
        m.IsMandatory = req.IsMandatory; m.Description = req.Description; m.IsActive = req.IsActive;
        await _db.SaveChangesAsync(); return Ok(MapModifier(m));
    }

    [HttpDelete("api/modifiers/{id:guid}")]
    [Authorize(Roles = "Admin,Underwriter")]
    public async Task<IActionResult> DeleteModifier(Guid id)
    {
        var m = await _db.Modifiers.FindAsync(id);
        if (m == null) return NotFound();
        _db.Modifiers.Remove(m); await _db.SaveChangesAsync(); return NoContent();
    }

    private static LimitDto MapLimit(Limit l) => new() { Id = l.Id, CoverId = l.CoverId, LimitType = l.LimitType, MinAmount = l.MinAmount, MaxAmount = l.MaxAmount, DefaultAmount = l.DefaultAmount, Currency = l.Currency, IsActive = l.IsActive };
    private static DeductibleDto MapDeductible(Deductible d) => new() { Id = d.Id, CoverId = d.CoverId, DeductibleType = d.DeductibleType, MinAmount = d.MinAmount, MaxAmount = d.MaxAmount, DefaultAmount = d.DefaultAmount, Currency = d.Currency, IsActive = d.IsActive };
    private static PremiumDto MapPremium(Premium p) => new() { Id = p.Id, CoverId = p.CoverId, PremiumType = p.PremiumType, BaseRate = p.BaseRate, FlatAmount = p.FlatAmount, MinPremium = p.MinPremium, CalculationBasis = p.CalculationBasis, Currency = p.Currency, IsActive = p.IsActive };
    private static ModifierDto MapModifier(Modifier m) => new() { Id = m.Id, CoverId = m.CoverId, ProductId = m.ProductId, Name = m.Name, Code = m.Code, ModifierType = m.ModifierType, ValueType = m.ValueType, MinValue = m.MinValue, MaxValue = m.MaxValue, DefaultValue = m.DefaultValue, IsMandatory = m.IsMandatory, Description = m.Description, IsActive = m.IsActive };
}
