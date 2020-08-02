using EnterpriseBot.VK.Extensions;
using EnterpriseBot.VK.Models.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace EnterpriseBot.VK
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.MigrateDatabase<ErrorDbContext>(throwExceptionInCaseOfFailure: true);

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
                        string prefix = $"{environment}_VK_";
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


        private static string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
