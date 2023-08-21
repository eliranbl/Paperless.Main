using Currency.Domain.ExchangeRates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Currency;

/// <summary>
/// Function.
/// </summary>
public class Function
{
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ILogger<Function> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="exchangeRateService"></param>
    public Function(IExchangeRateService exchangeRateService, ILogger<Function> logger)
    {
        _exchangeRateService = exchangeRateService;
        _logger = logger;
    }

    [FunctionName("Function")]
    public async Task<ActionResult<ExchangeRateResponse>> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "currency/{date}")] HttpRequest req,
        string date
      )
    {
     //   log.LogInformation($"Start function with parameter {date}");

        var regex = new Regex("^([0-9]{2})(0[1-9]|1[0-2])$");
        if (!regex.IsMatch(date))
        {
       //     log.LogError("Failed process regex");
            return new BadRequestObjectResult("Date have to be on format YYMM");
        }

        var response = await _exchangeRateService.GetByYearMonthDateAsync(date);
        dynamic dataResponse = JsonConvert.SerializeObject(response);

        return new OkObjectResult(dataResponse);
    }
}