namespace Currency.Domain.ExchangeRates;

/// <summary>
/// Exchange rate service.
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Get rate by year an month date async.
    /// </summary>
    /// <param name="date">Date on YYMM format</param>
    /// <returns>Exchange rate response.</returns>
    Task<ExchangeRateResponse> GetByYearMonthDateAsync(string date);
}