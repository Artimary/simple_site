using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<MouseData> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MouseData>(entity => 
            {
                entity.ToTable("MouseData");
                entity.Property(e => e.EventJson).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "DefaultConnection",
                    options => options.EnableRetryOnFailure()
                );
            }
        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}