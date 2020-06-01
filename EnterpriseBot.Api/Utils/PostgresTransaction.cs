using EnterpriseBot.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace EnterpriseBot.Api.Utils
{
    public class PostgresTransaction : IDisposable
    {
        public IDbContextTransaction Transaction { get; private set; }

        private readonly PostgresTransactionLimiter limiter;
        private bool disposed = false;

        public PostgresTransaction(PostgresTransactionLimiter limiter, DbContext context)
        {
            this.limiter = limiter;
            Transaction = context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Transaction.Dispose();
                limiter.ReleaseTransaction();
            }

            disposed = true;
        }

        ~PostgresTransaction()
        {
            Dispose(false);
        }
    }
}
