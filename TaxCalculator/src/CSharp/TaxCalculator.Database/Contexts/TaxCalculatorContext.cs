using Microsoft.EntityFrameworkCore;
using TaxCalculator.Database.Entities;

namespace TaxCalculator.Database.Contexts
{
    public class TaxCalculatorContext : DbContext
    {
        public TaxCalculatorContext(DbContextOptions<TaxCalculatorContext> options) : base(options)
        { 
        
        }
        public DbSet<CityEntity> Cities { get; set; }
        public DbSet<VehicleEntity> Vehicles { get; set; }
        public DbSet<PublicHolidayEntity> PublicHolidays { get; set; }
        public DbSet<TaxUnitEntity> TaxUnits { get; set; }
        public DbSet<CityVehicleTaxExemptionEntity> CityVehicleTaxExemptions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CityEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<VehicleEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<PublicHolidayEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.City)
                .WithMany(x => x.PublicHolidays);
            });

            modelBuilder.Entity<TaxUnitEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.City)
                .WithMany(x => x.TaxUnits);
            });

            modelBuilder.Entity<CityVehicleTaxExemptionEntity>(entity =>
            {
                entity.HasKey(x => new { x.CityId, x.VehicleId });

                entity.HasOne(x => x.City)
                .WithMany(x => x.CityVehicleTaxExemptions);
            });
        }
    }
}