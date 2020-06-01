using EnterpriseBot.Api.Utils;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Services
{
    public class PostgresTransactionLimiter
    {
        private readonly SemaphoreSlim throttler;

        public PostgresTransactionLimiter()
        {
            throttler = new SemaphoreSlim(1);
        }

        public async Task<PostgresTransaction> WaitOtherTransactionsAndBeginAsync(DbContext context)
        {
            await throttler.WaitAsync();
            return new PostgresTransaction(this, context);
        }

        public void ReleaseTransaction()
        {
            throttler.Release();
        }
    }
}
