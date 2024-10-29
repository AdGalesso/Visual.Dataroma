using Microsoft.EntityFrameworkCore;
using Visual.Dataroma.Domain.Contexts.Map;
using Visual.Dataroma.Domain.Entities;

namespace Visual.Dataroma.Domain.Contexts
{
    public class VisualDataromaContext : DbContext
    {
        public DbSet<Superinvestor> Superinvestors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=quantumPassw0rd;Database=visual.dataroma";

            optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SuperinvestorsMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
