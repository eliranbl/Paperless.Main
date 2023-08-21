namespace Currency.Domain.ExchangeRates;

/// <summary>
/// Exchange rate api request.
/// </summary>
public class ExchangeRateApiRequest
{
    /// <summary>
    /// From date.
    /// </summary>
    public string FromDate { get; set; }

    /// <summary>
    /// To date.
    /// </summary>
    public string ToDate { get; set; }
}