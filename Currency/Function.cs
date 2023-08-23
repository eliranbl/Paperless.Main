using System;
using Currency.Domain.ExchangeRates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Text;

namespace Currency;

/// <summary>
/// Function.
/// </summary>
public class Function
{
    private readonly IExchangeRateService _exchangeRateService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="exchangeRateService"></param>
    public Function(IExchangeRateService exchangeRateService )
    {
        _exchangeRateService = exchangeRateService;
    }

    [FunctionName("Function")]
    public async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "currency/{date}")] HttpRequest req,
        int date, ILogger logger)
    {
        try
        {
            logger.LogInformation($"Start function with parameter {date}");

            req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            var dateRequest = date.ToString();
            var regex = new Regex("^([0-2]{1})([0-9]{1})(0[1-9]|1[0-2])$");
            if (!regex.IsMatch(dateRequest))
            {
                logger.LogError("Failed process regex");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Date have to be on format YYMM, year start from 00",
                        Encoding.UTF8, "application/json")
                };
            }

            var response = await _exchangeRateService.GetByYearMonthDateAsync(dateRequest);
            dynamic dataResponse = JsonConvert.SerializeObject(response);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(dataResponse, Encoding.UTF8,
                    "application/json")
            };
        }
        catch (Exception e)
        {
            logger.LogError($"Failed get data from service {e.Message}");
             return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"Failed get data from service {e.Message}",
                        Encoding.UTF8, "application/json")
                };
            
        }
        
    }
}