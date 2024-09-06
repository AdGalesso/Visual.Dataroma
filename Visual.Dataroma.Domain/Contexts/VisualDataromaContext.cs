using Microsoft.EntityFrameworkCore;

namespace Visual.Dataroma.Domain.Contexts
{
    public class VisualDataromaContext : DbContext
    {
        public DbSet<Superinvestors> Superinvestors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;Database=visual.dataroma;User Id=sa;Password=quantumPassw0rd;Trust Server Certificate=true";

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
