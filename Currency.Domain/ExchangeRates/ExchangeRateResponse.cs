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
    public List<Dictionary<string,string>> Graph { get; set; }
}