using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace EnterpriseBot.Api.Extensions
{
    public static class HostExtensions
    {
        /// <summary>
        /// Creates the database if it doesn't already exist and applies any pending migrations for <see cref="DbContext"/> <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="DbContext"/> which migrations are to be applied</typeparam>
        /// <param name="throwExceptionInCaseOfFailure">A value indicating whether to tolerate exceptions during the migration or not</param>
        /// <remarks>This is an adoptation of the following code: https://github.com/dotnet/efcore/issues/9033#issuecomment-317104564</remarks>
        public static IHost MigrateDatabase<T>(this IHost host, bool throwExceptionInCaseOfFailure = false) where T : DbContext
        {
            using var scope = host.Services.CreateScope();
            var scopeServices = scope.ServiceProvider;

            try
            {
                var databaseContext = scopeServices.GetRequiredService<T>();
                databaseContext.Database.Migrate();
            }
            catch(Exception ex)
            {
                var loggerFactory = scopeServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(typeof(HostExtensions));
                logger.LogError(ex, "An error occurred while migrating the database.");

                if (throwExceptionInCaseOfFailure)
                {
                    throw ex;
                }
            }

            return host;
        }
    }
}
