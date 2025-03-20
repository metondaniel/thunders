using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Domain.Aggregates;
using Thunders.TechTest.Domain;

namespace Thunders.TechTest.ApiService.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TollUsage> TollUsages { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TollUsage>(entity =>
            {
                entity.ToTable("TollUsages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlazaId).HasMaxLength(20);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(2);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.ReportType).HasMaxLength(50);
                entity.Property(r => r.Data).HasColumnType("nvarchar(max)");
            });
        }
    }
}
