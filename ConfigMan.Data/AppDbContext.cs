using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigMan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<DeploymentEnvironment> Environments { get; set; } 
        public DbSet<Application> Applications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeploymentEnvironment>(entity =>
            {
                entity.HasKey(e => e.Name);
                entity.Property(x => x.Settings);
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(a => a.Name);
                entity.Property(a => a.ApplicationDefaults);
                entity.Property(a => a.EnvironmentSettings);
            });

        }
    }
}