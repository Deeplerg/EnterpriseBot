using EnterpriseBot.Background.Models.Contexts;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EnterpriseBot.Background
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();


            string hangfireConnection = Configuration.GetConnectionString("HangfireDb");


            // Is used only for the creation of the database since Hangfire doesn't do it.

#pragma warning disable CS0618 // Type or member is obsolete
            services.AddDbContext<HangfireContext>(opt =>
#pragma warning restore CS0618 // Type or member is obsolete
            {
                opt.UseNpgsql(hangfireConnection);
            });


            services.AddHangfire((config) =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseRecommendedSerializerSettings()
                      .UseDefaultActivator()
                      .UsePostgreSqlStorage(hangfireConnection);
            });
            //services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseHangfireServer();

            // For now, it's better to disable this for security purposes.
            //app.UseHangfireDashboard();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
