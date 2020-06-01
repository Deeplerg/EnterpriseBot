using EnterpriseBot.VK.Extensions;
using EnterpriseBot.VK.Models.Contexts;
using EnterpriseBot.VK.Models.Settings;
using EnterpriseBot.VK.MvcInputFormatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
//using VkNet.AudioBypassService.Extensions;

namespace EnterpriseBot.VK
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
            services.Configure<VkSettings>(Configuration.GetSection(nameof(VkSettings)));
            services.Configure<ApiSettings>(Configuration.GetSection(nameof(ApiSettings)));

            //services.AddAudioBypass();
            services.AddVkApi();

            services.AddEntbotApi();
            services.AddMenuMapper();
            services.AddVkMessageGateway();
            services.AddMenuRouter();
            services.AddLocalPlayerManager();

            services.AddVkUpdateHandler();


            services.AddDbContext<ErrorDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("ErrorsSQLite"));
            });


            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new GroupUpdateInputFormatter());
            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: true,
                                                                 overrideSpecifiedNames: false)
                };
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "vk-callback",
                    pattern: "Callback/{id?}",
                    defaults: new
                    {
                        controller = "Vk",
                        action = "Callback"
                    });
            });
        }
    }
}
