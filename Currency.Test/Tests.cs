using Currency.Domain.ExchangeRates;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

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

            _function = new Function(host.Services.GetRequiredService<IExchangeRateService>());
      
        }

        [Test]
        public async Task SendCorrectDateAndGetResponseAsync()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());
            var logger = Mock.Of<ILogger>();

            // act
            var response = await _function.Run(req, 2012, logger);

            // assert
            var result = response.Result as OkObjectResult;
            result.StatusCode.Should().Be(200);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task SendWrongDateAndGetResponseAsync()
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext());
            var logger = Mock.Of<ILogger>();
            // act
            var response = await _function.Run(req, 2013, logger);

            // assert
            var result = response.Result as BadRequestObjectResult;
            result.StatusCode.Should().NotBe(200);
        }
    }
}