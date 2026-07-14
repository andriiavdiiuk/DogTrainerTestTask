using DogTrainerTestTask.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DogTrainerTestTask.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, UserRole, long>(options)
{
    public DbSet<Litter> Litters { get; set; }
    public DbSet<BreederBenefit>  BreederBenefits { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Litter>(e =>
        {
            e.HasKey(p => p.Id);
            
            e.HasOne(p => p.Breeder)
                .WithMany(p => p.Litters)
                .HasForeignKey(p => p.BreederId)
                .IsRequired();

            e.Property(p => p.Status).IsRequired();
            
            e.Property(p => p.CreatedAt).IsRequired();
        });

        builder.Entity<BreederBenefit>(e =>
        {
            e.HasKey(p => p.BreederId);

            e.HasOne(p => p.Breeder)
                .WithOne(p => p.BreederBenefit)
                .HasForeignKey<BreederBenefit>(p => p.BreederId)
                .IsRequired();

            e.Property(p => p.FreeLimit).IsRequired();
            
            e.Property(p => p.UsedCount).IsRequired();
        });

        builder.Entity<AuditLog>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasOne(p => p.ModifiedByUser)
                .WithMany(p => p.AuditLogs)
                .HasForeignKey(p => p.ModifiedBy);
            
            e.Property(p => p.EntityId).IsRequired();
            e.Property(p => p.EntityName).IsRequired();
            e.Property(p => p.Action).IsRequired();
            e.Property(p => p.CreatedAt).IsRequired();
        });
    }
}