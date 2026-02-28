using ProductBuilder.Application.DTOs.Quotes;

namespace ProductBuilder.Application.Interfaces;

public interface IPremiumCalculationService
{
    Task<QuoteDto> CalculateAsync(Guid quoteId);
}
