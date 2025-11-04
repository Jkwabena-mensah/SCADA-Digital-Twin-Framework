using Microsoft.EntityFrameworkCore;
using WebController.Models;

namespace WebController.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SensorReading> SensorReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorReading>()
                .HasIndex(r => r.Timestamp);

            modelBuilder.Entity<SensorReading>()
                .HasIndex(r => r.AssetId);
        }
    }
}