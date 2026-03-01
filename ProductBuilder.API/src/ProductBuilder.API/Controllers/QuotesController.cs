using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Quotes;
using ProductBuilder.Application.Interfaces;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;
using System.Security.Claims;

namespace ProductBuilder.API.Controllers;

[ApiController]
[Route("api/quotes")]
[Authorize]
public class QuotesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPremiumCalculationService _calcService;

    public QuotesController(AppDbContext db, IPremiumCalculationService calcService)
    {
        _db = db;
        _calcService = calcService;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuoteDto>>> GetAll([FromQuery] string? status)
    {
        var query = _db.Quotes.Include(q => q.Product).Include(q => q.Broker).Include(q => q.Underwriter).ThenInclude(u => u!.User).AsQueryable();
        if (!string.IsNullOrEmpty(status)) query = query.Where(q => q.Status == status);
        var quotes = await query.ToListAsync();
        return Ok(quotes.Select(q => MapToDto(q)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuoteDto>> GetById(Guid id)
    {
        var q = await _db.Quotes
            .Include(q => q.Product)
            .Include(q => q.Broker)
            .Include(q => q.Underwriter).ThenInclude(u => u!.User)
            .Include(q => q.QuoteCovers).ThenInclude(qc => qc.Cover)
            .Include(q => q.QuoteModifiers).ThenInclude(qm => qm.Modifier)
            .FirstOrDefaultAsync(q => q.Id == id);
        return q == null ? NotFound() : Ok(MapToFullDto(q));
    }

    [HttpPost]
    public async Task<ActionResult<QuoteDto>> Create([FromBody] CreateQuoteRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var quote = new Quote
        {
            ProductId = request.ProductId, BrokerId = request.BrokerId, UnderwriterId = request.UnderwriterId,
            InsuredName = request.InsuredName, InsuredEmail = request.InsuredEmail, InsuredPhone = request.InsuredPhone,
            Currency = request.Currency, ValidUntil = request.ValidUntil, Notes = request.Notes, CreatedBy = userId
        };
        _db.Quotes.Add(quote);

        foreach (var c in request.Covers)
            _db.QuoteCovers.Add(new QuoteCover { QuoteId = quote.Id, CoverId = c.CoverId, IsSelected = c.IsSelected, SelectedLimit = c.SelectedLimit, SelectedDeductible = c.SelectedDeductible, BasisValue = c.BasisValue });

        foreach (var m in request.Modifiers)
            _db.QuoteModifiers.Add(new QuoteModifier { QuoteId = quote.Id, ModifierId = m.ModifierId, AppliedValue = m.AppliedValue });

        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = quote.Id }, MapToDto(quote));
    }

    [HttpPost("{id:guid}/calculate")]
    public async Task<ActionResult<QuoteDto>> Calculate(Guid id)
    {
        try { return Ok(await _calcService.CalculateAsync(id)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<QuoteDto>> Update(Guid id, [FromBody] UpdateQuoteRequest request)
    {
        var quote = await _db.Quotes
            .Include(q => q.Product).Include(q => q.Broker)
            .Include(q => q.Underwriter).ThenInclude(u => u!.User)
            .Include(q => q.QuoteCovers).ThenInclude(qc => qc.Cover)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (quote == null) return NotFound();
        if (quote.Status != "Draft") return BadRequest(new { message = "Only draft quotes can be edited" });

        quote.BrokerId = request.BrokerId;
        quote.UnderwriterId = request.UnderwriterId;
        quote.InsuredName = request.InsuredName;
        quote.InsuredEmail = request.InsuredEmail;
        quote.InsuredPhone = request.InsuredPhone;
        quote.Currency = request.Currency;
        quote.ValidUntil = request.ValidUntil;
        quote.Notes = request.Notes;
        quote.UpdatedAt = DateTime.UtcNow;

        foreach (var c in request.Covers)
        {
            var qc = quote.QuoteCovers.FirstOrDefault(x => x.CoverId == c.CoverId);
            if (qc == null) continue;
            qc.IsSelected = c.IsSelected;
            qc.BasisValue = c.BasisValue;
            qc.SelectedLimit = c.SelectedLimit;
            qc.SelectedDeductible = c.SelectedDeductible;
            qc.CalculatedPremium = null;
        }

        await _db.SaveChangesAsync();
        return Ok(MapToFullDto(quote));
    }

    [HttpPut("{id:guid}/submit")]
    public async Task<ActionResult<QuoteDto>> Submit(Guid id)
    {
        var quote = await _db.Quotes.Include(q => q.Product).Include(q => q.Broker).Include(q => q.Underwriter).ThenInclude(u => u!.User).FirstOrDefaultAsync(q => q.Id == id);
        if (quote == null) return NotFound();
        if (quote.Status != "Draft") return BadRequest(new { message = "Only draft quotes can be submitted" });
        quote.Status = "Submitted";
        quote.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(quote));
    }

    [HttpPut("{id:guid}/bind")]
    public async Task<ActionResult<QuoteDto>> Bind(Guid id)
    {
        var quote = await _db.Quotes.Include(q => q.Product).Include(q => q.Broker).Include(q => q.Underwriter).ThenInclude(u => u!.User).FirstOrDefaultAsync(q => q.Id == id);
        if (quote == null) return NotFound();
        if (quote.Status != "Submitted") return BadRequest(new { message = "Only submitted quotes can be bound" });
        quote.Status = "Bound";
        quote.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(quote));
    }

    [HttpPut("{id:guid}/revise")]
    public async Task<ActionResult<QuoteDto>> Revise(Guid id)
    {
        var quote = await _db.Quotes.Include(q => q.Product).Include(q => q.Broker).Include(q => q.Underwriter).ThenInclude(u => u!.User).FirstOrDefaultAsync(q => q.Id == id);
        if (quote == null) return NotFound();
        if (quote.Status != "Submitted") return BadRequest(new { message = "Only submitted quotes can be revised" });
        quote.Status = "Draft";
        quote.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(quote));
    }

    [HttpPut("{id:guid}/revise-bind")]
    public async Task<ActionResult<QuoteDto>> ReviseBind(Guid id)
    {
        var quote = await _db.Quotes.Include(q => q.Product).Include(q => q.Broker).Include(q => q.Underwriter).ThenInclude(u => u!.User).FirstOrDefaultAsync(q => q.Id == id);
        if (quote == null) return NotFound();
        if (quote.Status != "Bound") return BadRequest(new { message = "Only bound quotes can have their binding revised" });
        quote.Status = "Draft";
        quote.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(MapToDto(quote));
    }

    private static QuoteDto MapToDto(Quote q) => new()
    {
        Id = q.Id, ProductId = q.ProductId, ProductName = q.Product?.Name ?? string.Empty,
        BrokerId = q.BrokerId, BrokerName = q.Broker?.CompanyName,
        UnderwriterId = q.UnderwriterId,
        UnderwriterName = q.Underwriter != null ? $"{q.Underwriter.User?.FirstName} {q.Underwriter.User?.LastName}" : null,
        InsuredName = q.InsuredName, InsuredEmail = q.InsuredEmail, InsuredPhone = q.InsuredPhone,
        Status = q.Status, Currency = q.Currency, BasePremium = q.BasePremium, TotalPremium = q.TotalPremium,
        ValidUntil = q.ValidUntil, Notes = q.Notes, CreatedAt = q.CreatedAt
    };

    private static QuoteDto MapToFullDto(Quote q)
    {
        var dto = MapToDto(q);
        dto.Covers = q.QuoteCovers.Select(qc => new QuoteCoverDto { Id = qc.Id, CoverId = qc.CoverId, CoverName = qc.Cover.Name, IsSelected = qc.IsSelected, SelectedLimit = qc.SelectedLimit, SelectedDeductible = qc.SelectedDeductible, CalculatedPremium = qc.CalculatedPremium, BasisValue = qc.BasisValue }).ToList();
        dto.Modifiers = q.QuoteModifiers.Select(qm => new QuoteModifierDto { Id = qm.Id, ModifierId = qm.ModifierId, ModifierName = qm.Modifier.Name, AppliedValue = qm.AppliedValue, PremiumImpact = qm.PremiumImpact }).ToList();
        return dto;
    }
}
