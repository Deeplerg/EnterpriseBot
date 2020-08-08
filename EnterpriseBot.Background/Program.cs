using EnterpriseBot.Background.Models.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;

namespace EnterpriseBot.Background
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var scopeServices = scope.ServiceProvider;

                try
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    var databaseContext = scopeServices.GetRequiredService<HangfireContext>();
#pragma warning restore CS0618 // Type or member is obsolete
                    databaseContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = scopeServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");

                    throw ex;
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostBuilder, configBuilder) =>
                {
                    string environment = hostBuilder.HostingEnvironment.EnvironmentName;

                    configBuilder.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);

                    configBuilder.AddEnvironmentVariables("ASPNETCORE_");
                    configBuilder.AddEnvironmentVariables("DOTNET_");
                    configBuilder.AddEnvironmentVariables();

                    configBuilder.AddCommandLine(args);


                    string secretsPath = configBuilder.Build().GetValue<string>("SecretsPath");
                    if (!string.IsNullOrEmpty(secretsPath))
                    {
                        string prefix = $"{environment}_Background_";

                        configBuilder.AddKeyPerFile(directoryPath: secretsPath, filePrefix: prefix);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((hostBuilderContext, loggerConfig) =>
                {
                    loggerConfig.ReadFrom.Configuration(hostBuilderContext.Configuration);
                });
    }
}
