using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebApi
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = "SampleWebApi";
        // public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static readonly string EnvironmentPrefix = "SampleWebApi_";
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            var logConfig =  new LoggerConfiguration();
            ConfigureSerilog(logConfig, configuration, AppName);
            Log.Logger = logConfig.CreateBootstrapLogger();

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                using (var host = CreateHostBuilder(args, configuration).Build())
                {
                    // Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                    // host.MigrateDbContext<FirmasContext>((context, services) =>
                    // {
                    //     var env = services.GetService<IWebHostEnvironment>();
                    //     var logger = services.GetService<ILogger<FirmasContextSeed>>();

                    //     new FirmasContextSeed()
                    //         .SeedAsync(context, env, logger)
                    //         .Wait();
                    // })
                    // .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                    Log.Information("Starting web host ({ApplicationContext})...", AppName);
                    host.Run();
                }
            }
            catch (Exception e)
            {
                // Happens rarely but when it does, you'll thank me. :)
                Log.Logger.Fatal(e, "Unable to bootstrap web app.");
            }

            // Make sure all the log sinks have processed the last log before closing the application.
            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostContext, config) =>
                        config.AddConfiguration(configuration));
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, services, logConfig) => 
                {
                    ConfigureSerilog(logConfig, context.Configuration, AppName);

                    // if (bool.TryParse(context.Configuration["ApplicationInsights:EnableTelemetry"], out bool enabled) && enabled)
                    //     logConfig.WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);
                });

        private static IConfigurationRoot GetConfiguration()
         {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables(EnvironmentPrefix);
            var config = builder.Build();

            // if (config.GetValue<bool>("UseVault", false))
            // {
            //     builder.AddAzureKeyVault(
            //         $"https://{config["Vault:Name"]}.vault.azure.net/",
            //         config["Vault:ClientId"],
            //         config["Vault:ClientSecret"]);
            // }

            return builder.Build();
        }

        private static void ConfigureSerilog(LoggerConfiguration loggerConfiguration, IConfiguration configuration, string appName)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            if (!string.IsNullOrWhiteSpace(seqServerUrl))
                loggerConfiguration.WriteTo.Seq(seqServerUrl);

            loggerConfiguration
                .Enrich.WithProperty("AppName", appName)
                .ReadFrom.Configuration(configuration);
#if DEBUG
            // Used to filter out potentially bad data due debugging.
            // Very useful when doing Seq dashboards and want to remove logs under debugging session.
            loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
            // When using ".UseSerilog()" it will use "Log.Logger".
        }
    }
}