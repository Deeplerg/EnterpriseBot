using Microsoft.EntityFrameworkCore;
using System;

namespace EnterpriseBot.Background.Models.Contexts
{
    [Obsolete("Should only be used for the creation of the database for Hangfire")]
    public class HangfireContext : DbContext
    {
        public HangfireContext(DbContextOptions options) : base(options) { }
    }
}
