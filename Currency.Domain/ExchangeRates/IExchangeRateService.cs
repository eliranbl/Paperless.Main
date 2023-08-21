namespace Currency.Domain.ExchangeRates;

public interface IExchangeRateService
{
    Task<ExchangeRateResponse> GetByYearMonthDateAsync(string date);
}