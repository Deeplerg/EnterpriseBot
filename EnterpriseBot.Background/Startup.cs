using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
            var activator = new DependencyJobActivator(services.BuildServiceProvider());
            services.AddControllers();
            services.AddHangfire((config) =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseRecommendedSerializerSettings()
                      .UseDefaultActivator()
                      .UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireDb"));
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
            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class DependencyJobActivator : JobActivator
    {
        private readonly IServiceProvider serviceProvider;

        public DependencyJobActivator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type jobType)
        {
            return ActivatorUtilities.CreateInstance(serviceProvider, jobType);
        }

        public override JobActivatorScope BeginScope(PerformContext context)
        {
            var scope = serviceProvider.CreateScope();
            return new DependencyJobActivatorScope(scope);
        }
    }

    public class DependencyJobActivatorScope : JobActivatorScope
    {
        private readonly IServiceScope serviceScope;

        public DependencyJobActivatorScope(IServiceScope serviceScope)
        {
            this.serviceScope = serviceScope;
        }

        public override object Resolve(Type type)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(serviceScope.ServiceProvider, type);
        }

        public override void DisposeScope()
        {
            serviceScope.Dispose();
        }
    }

    public interface IJob<TParams> where TParams : class
    {
        void Execute(TParams pars);
    }

    public interface INewJob : IJob<NewJobParams>
    {
    }

    public class NewJobParams
    {
        public string Something { get; set; }
    }

    public class NewJob : INewJob
    {
        public void Execute(NewJobParams pars)
        {
            Console.WriteLine(pars.Something);
        }
    }
}
