namespace Currency.Domain.ExchangeRates;

/// <summary>
/// Exchange rate.
/// </summary>
public class ExchangeRate
{
    /// <summary>
    /// Day of month.
    /// </summary>
    public int DayOfMonth { get; set; }

    /// <summary>
    /// Rate.
    /// </summary>
    public float Rate { get; set; }
}