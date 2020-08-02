using EnterpriseBot.Api.Extensions;
using EnterpriseBot.Api.Models.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EnterpriseBot.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.MigrateDatabase<ApplicationContext>(throwExceptionInCaseOfFailure: true);

#pragma warning disable CS0618 // Type or member is obsolete
            host.MigrateDatabase<HangfireContext>(throwExceptionInCaseOfFailure: true);
#pragma warning restore CS0618 // Type or member is obsolete

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostBuilder, configBuilder) =>
                {
                    string environment = hostBuilder.HostingEnvironment.EnvironmentName;

                    configBuilder.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);

                    configBuilder.AddJsonFile($"GameSettings.{environment}.json", optional: false, reloadOnChange: true);

                    configBuilder.AddEnvironmentVariables("ASPNETCORE_");
                    configBuilder.AddEnvironmentVariables("DOTNET_");
                    configBuilder.AddEnvironmentVariables();

                    configBuilder.AddCommandLine(args);


                    string secretsPath = configBuilder.Build().GetValue<string>("SecretsPath");
                    if (!string.IsNullOrEmpty(secretsPath))
                    {
                        string prefix = $"{environment}_Api_";
                        configBuilder.AddKeyPerFile(conf =>
                        {
                            conf.FileProvider = new PhysicalFileProvider(secretsPath);
                            conf.IgnorePrefix = prefix;
                            conf.Optional = false;
                        });
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
