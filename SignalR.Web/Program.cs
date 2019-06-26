using System;
using SolarLab.BusManager.Abstraction;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SignalR.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args)
                .UseSerilog()
                .Build();

            var services = host.Services;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Signalr starting");
            try
            {
                logger.LogInformation("Connect to bus");
                var busManager = services.GetService<IBusManager>();
                busManager.StartBus(ServiceBusConfigurator.GetBusConfigurations(services));
            }
            catch (Exception ex)
            {
                logger.LogError("Error during start bus for Signalr", ex);
            }
            logger.LogInformation("Bus connection done");

            logger.LogInformation("Service SignalR started");
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
                    config.AddJsonFile("secrets/appsettings.secrets.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSerilog();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
