using EnterpriseBot.VK.Models.Other;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseBot.VK.Models.Contexts
{
    public class ErrorDbContext : DbContext
    {
        public DbSet<Error> Errors { get; set; }

        public ErrorDbContext(DbContextOptions options) : base(options) { }
    }
}
