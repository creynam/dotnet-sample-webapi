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
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);
#if DEBUG
            // Used to filter out potentially bad data due debugging.
            // Very useful when doing Seq dashboards and want to remove logs under debugging session.
            loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
            // When using ".UseSerilog()" it will use "Log.Logger".
            Log.Logger = loggerConfiguration.CreateLogger();
            try
            {
                using (var host = CreateHostBuilder(args, configuration).Build())
                {
                    // using (var scope = host.Services.CreateScope())
                    // {
                    //     var services = scope.ServiceProvider;
                    //     var logger = services.GetRequiredService<ILogger<EscriturasContextSeed>>();
                    //     logger.LogInformation("Conectando a BD {connStr}", configuration.GetConnectionString("EscriturasConnection"));

                    //     var env = services.GetService<IHostEnvironment>();
                    //     var context = services.GetService<EscriturasContext>();
                    //     new EscriturasContextSeed()
                    //         .SeedAsync(context, env, logger)
                    //         .Wait();
                    // }
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
                    webBuilder.UseSerilog();
                });
    }
}