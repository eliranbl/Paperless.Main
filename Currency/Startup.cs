using Currency.Domain.ExchangeRates;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(Currency.Startup))]
namespace Currency
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<ExchangeRateSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ExchangeRateSettings").Bind(settings);
                });
            //Register HttpClientFactory
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IExchangeRateService, ExchangeRateService>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);

            var context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(
                    Path.Combine(context.ApplicationRootPath, "appsettings.json"),
                    optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    }
}
