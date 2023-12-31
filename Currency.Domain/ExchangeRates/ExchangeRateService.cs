﻿using Microsoft.Extensions.Options;
using System.Text;

namespace Currency.Domain.ExchangeRates
{
    /// <summary>
    /// Exchange rate service.
    /// </summary>
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ExchangeRateSettings _settings;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="httpClient">Http client.</param>
        public ExchangeRateService(IOptions<ExchangeRateSettings> settings, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        /// <summary>
        /// Get rate by year an month date async.
        /// </summary>
        /// <param name="date">Date on YYMM format</param>
        /// <returns>Exchange rate response.</returns>
        public async Task<ExchangeRateResponse> GetByYearMonthDateAsync(string date)
        {
            try
            {
                var exchangeRateResponse = new ExchangeRateResponse();
                var exchangeRateApiRequest = GetExchangeRateApiRequest(date);

                if (exchangeRateApiRequest is null)
                    throw new Exception("Failed convert date to ExchangeRateResponse object");

                var exchangeRates = await GetExchangeRates(exchangeRateApiRequest);
                if (!exchangeRates.Any())
                    throw new Exception($"No data on this date, please check your date");

                var dictionaryResult = exchangeRates.ToDictionary(item => item.DayOfMonth.ToString(), item => item.Rate.ToString());
                dictionaryResult.Add("Min", exchangeRates.Min(x => x.Rate).ToString());
                dictionaryResult.Add("Max", exchangeRates.Max(x => x.Rate).ToString());

                exchangeRateResponse.Graph = new List<Dictionary<string, string>> { dictionaryResult };

                return exchangeRateResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed GetByYearMonthDateAsync {e.Message}");
            }

        }

        #region Private

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

            stringBuilder.Append($"20{year}");

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

        #endregion

    }
}