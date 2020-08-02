using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.MvcInputFormatters;
using EnterpriseBot.Api.Routing;
using EnterpriseBot.Api.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
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
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

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
            //int poolSize = Configuration.GetValue<int>("DbContextPoolSize"); //this won't throw an exception while getting invalid value
            int poolSize = int.Parse(Configuration.GetValue<string>("DbContextPoolSize")); //but this will

            services.AddDbContextPool<ApplicationContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("Game"));
                opt.UseLazyLoadingProxies();
            }, poolSize);
            #endregion

            #region Hangfire
            string hangfireConnection = Configuration.GetConnectionString("Hangfire");

            // Is used only for the creation of the database since Hangfire doesn't do it.

#pragma warning disable CS0618 // Type or member is obsolete
            services.AddDbContext<HangfireContext>(opt =>
#pragma warning restore CS0618 // Type or member is obsolete
            {
                opt.UseNpgsql(hangfireConnection);
            });


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
                        //opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        opt.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
                    });

            services.AddTransient<IBackgroundJobScheduler, BackgroundJobScheduler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                    PreserveReferencesHandling = PreserveReferencesHandling.All
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
