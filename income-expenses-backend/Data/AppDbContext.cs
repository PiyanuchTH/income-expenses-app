using IncomeExpenses.Models;
using Microsoft.EntityFrameworkCore;

namespace IncomeExpenses.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Income> Transaction { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
