using Currency.Domain.ExchangeRates;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Currency.Test
{
    public class Tests
    {
        private Function _function;

        [SetUp]
        public void Setup()
        {
            var startup = new Startup();
            var host = new HostBuilder()
                .ConfigureWebJobs(startup.Configure)
                .Build();

            _function = new Function(host.Services.GetRequiredService<IExchangeRateService>(),
                host.Services.GetRequiredService<ILogger<Function>>());
        }

        [Test]
        public async Task SendCorrectDateAndGetResponseAsync()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());

            // act
            var response = await _function.Run(req, "2012");

            // assert
            var result = response.Result as OkObjectResult;
            result.StatusCode.Should().Be(200);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task SendWorngDateAndGetResponseAsync()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());

            // act
            var response = await _function.Run(req, "2013");

            // assert
            var result = response.Result as BadRequestObjectResult;
            result.StatusCode.Should().NotBe(200);
        }
    }
}
