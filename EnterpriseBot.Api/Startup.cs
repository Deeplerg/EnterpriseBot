using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.MvcInputFormatters;
using EnterpriseBot.Api.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace EnterpriseBot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

            builder.AddConfiguration(configuration);

            builder.AddJsonFile("GameplaySettings.json")
                   .AddJsonFile("LocalizationSettings.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Options
            services.Configure<GameplaySettings>(Configuration.GetSection("GameplaySettings"));
            services.Configure<LocalizationSettings>(Configuration.GetSection("LocalizationSettings"));
            #endregion

            services.AddSingleton<PostgresTransactionLimiter>();

            #region Database
            int poolSize = 256;
            services.AddDbContextPool<ApplicationContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("GameDb"));
                opt.UseLazyLoadingProxies();
            }, poolSize);
            #endregion

            #region Hangfire
            string hangfireConnection = Configuration.GetConnectionString("HangfireDb");
            services.AddHangfire(configuration => configuration
                                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                 .UseDefaultActivator()
                                 .UseRecommendedSerializerSettings()
                                 .UsePostgreSqlStorage(hangfireConnection));
            #endregion

            services.AddMvc(opt => opt.InputFormatters.Insert(0, new RawJsonBodyInputFormatter()))
                    .AddNewtonsoftJson(opt =>
                    {
                        opt.SerializerSettings.ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: true, overrideSpecifiedNames: false)
                        };
                        opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseExceptionHandler(app =>
            {
                app.Run(ExceptionHandler);
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapAreaControllerRoute(
                    name: "business_area",
                    areaName: nameof(Areas.Business),
                    pattern: "{area:exists}/{controller}/{action}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "crafting_area",
                    areaName: nameof(Areas.Crafting),
                    pattern: "{area:exists}/{controller}/{action}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "essences_area",
                    areaName: nameof(Areas.Essences),
                    pattern: "{area:exists}/{controller}/{action}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "storages_area",
                    areaName: nameof(Areas.Storages),
                    pattern: "{area:exists}/{controller}/{action}/{id?}");
            });
        }

        private async Task ExceptionHandler(HttpContext ctx)
        {
            var ex = ctx.Features.Get<IExceptionHandlerFeature>().Error;

            var errorGameResult = new EmptyGameResult()
            {
                LocalizedError = new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Unknown error occurred. {ex}",
                    RussianMessage = $"Произошла неизвестная ошибка. {ex}"
                }
            };

            try
            {
                string serializedError = JsonConvert.SerializeObject(errorGameResult, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: true, overrideSpecifiedNames: false)
                    },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                await ctx.Response.WriteAsync(serializedError);
            }
            catch
            {
                await ctx.Response.WriteAsync("Unknown error occurred. Serializing the error was not possible.");
            }
        }
    }
}
