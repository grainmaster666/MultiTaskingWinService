using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiTaskingWinService.Helper;
using MultiTaskingWinService.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTaskingWinService
{
    internal class Program
    {
        protected Program() { }

        private static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                   services.Configure<MailSettings>(Configuration().GetSection("MailSettings"));
                    //services.AddHostedService<DealJobService>();
                    //services.AddHostedService<UpdateOtherParametersJob>();
                    //services.AddHostedService<UpdateScreenUrlJob>();
                    //services.AddHostedService<StockHistoricalDataJob>();
                    //services.AddHostedService<EmailService>();
                    //services.AddHostedService<TickerTapJob>();
                    services.AddHostedService<CryptoService>();
                });

            if (isService)
            {
                await builder.RunAsServiceAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }
        }
        public static IConfigurationRoot Configuration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.development.json", optional: true)
                .Build();

            return builder;

        }
    }
}
