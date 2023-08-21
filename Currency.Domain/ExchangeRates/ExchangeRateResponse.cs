using Newtonsoft.Json;

namespace Currency.Domain.ExchangeRates;

/// <summary>
/// Exchange rate response.
/// </summary>
public class ExchangeRateResponse
{
    /// <summary>
    /// Graph.
    /// </summary>
    [JsonProperty("GRAPH")]
    public List<ExchangeRate> Graph { get; set; }
    
    /// <summary>
    /// Min value.
    /// </summary>
    public float Min { get; set; }

    /// <summary>
    /// Max value.
    /// </summary>
    public float Max { get; set; }
}