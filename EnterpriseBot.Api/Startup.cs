using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.MarketSettings;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings;
using EnterpriseBot.Api.Models.Settings.BusinessSettings;
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
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using Microsoft.Extensions.Options;
using EnterpriseBot.Api.Models.Settings;
using Microsoft.AspNetCore.Mvc.Razor;
using EnterpriseBot.Api.Routing;

namespace EnterpriseBot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

            builder.AddConfiguration(configuration);

            builder.AddJsonFile("GameSettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Options
            services.Configure<GameSettings>(Configuration.GetSection("GameSettings"));

            services.Configure<RazorViewEngineOptions>(opt =>
            {
                opt.ViewLocationExpanders.Add(new SubAreaViewLocationExpander());
            });
            #endregion

            services.AddSingleton<PostgresTransactionLimiter>();

            #region Database
            int poolSize = 128;
            services.AddDbContextPool<ApplicationContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("Game"));
                opt.UseLazyLoadingProxies();
            }, poolSize);
            #endregion

            #region Hangfire
            string hangfireConnection = Configuration.GetConnectionString("Hangfire");
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

            //app.UseAuthorization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "game_areas",
                    pattern: "{area:exists}/{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                    name: "game_subareas",
                    pattern: "{area:exists}/{subarea:exists}/{controller}/{action}/{id?}");

                endpoints.MapDefaultControllerRoute();
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
