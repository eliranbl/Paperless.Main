using Microsoft.Extensions.Options;
using System.Text;

namespace Currency.Domain.ExchangeRates
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ExchangeRateSettings _settings;
        private readonly HttpClient _httpClient;
    
        public ExchangeRateService(IOptions<ExchangeRateSettings> settings, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }
        
        public async Task<ExchangeRateResponse> GetByYearMonthDateAsync(string date)
        {
            var exchangeRateApiRequest = GetExchangeRateApiRequest(date);

            if (exchangeRateApiRequest is null)
            {
                throw new Exception("Failed convert date to ExchangeRateResponse object");
            }

            var exchangeRates = await GetExchangeRates(exchangeRateApiRequest);

            var exchangeRateResponse = new ExchangeRateResponse
            {
                Graph = exchangeRates,
                Min = exchangeRates.Min(x => x.Rate),
                Max = exchangeRates.Max(x => x.Rate)
            };

            return exchangeRateResponse;
        }

        private async Task<List<ExchangeRate>> GetExchangeRates(ExchangeRateApiRequest request)
        {
            var exchangeRateResponses = new List<ExchangeRate>();

            var response = await _httpClient.GetAsync
                ($"{_settings.Url}/{request.FromDate}/{request.ToDate}/usd/ILS");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed get data form API {await response.Content.ReadAsStringAsync()}");

            var data = await response.Content.ReadAsStringAsync();
            using var reader = new StringReader(data);
            while (await reader.ReadLineAsync() is { } line)
            {
                var lineData = SplitWhitespace(line);

                exchangeRateResponses.Add(new ExchangeRate
                {
                    DayOfMonth = DateOnly.Parse(lineData[0]).Day,
                    Rate = float.Parse(lineData[1])
                });
            }
            return exchangeRateResponses;
        }

        private string[] SplitWhitespace(string input)
        {
            var whitespace = new[] { ' ', '\t' };
            return input.Split(whitespace);
        }

        private ExchangeRateApiRequest? GetExchangeRateApiRequest(string date)
        {
            var stringBuilder = new StringBuilder();
            var offset = Math.Min(2, date.Length);
            var year = date.Substring(0, offset);
            var month = date.Substring(2, offset);

            if (year.StartsWith("9"))
            {
                stringBuilder.Append($"19{year}");
            }
            else
            {
                stringBuilder.Append($"20{year}");
            }

            year = stringBuilder.ToString();
            stringBuilder.Clear();

            stringBuilder.Append($"{year}-{month}-01");
            var dateString = stringBuilder.ToString();

            if (DateOnly.TryParse(dateString, out DateOnly DateConverter))
            {
                var request = new ExchangeRateApiRequest
                {
                    FromDate = DateConverter.ToString("yyyy-MM-dd")
                };

                var daysInMonth = DateTime.DaysInMonth(int.Parse(year), int.Parse(month));
                var lastDayOfMonth = new DateTime(int.Parse(year), int.Parse(month), daysInMonth);
                request.ToDate = lastDayOfMonth.ToString("yyyy-MM-dd");

                return request;
            }
            return null;
        }
    }
}