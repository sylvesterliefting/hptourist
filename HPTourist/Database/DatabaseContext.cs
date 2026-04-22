using HPTourist.Models;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.EhicNumber).IsRequired().HasMaxLength(20);
            entity.Property(p => p.Email).IsRequired().HasMaxLength(254);
            entity.Property(p => p.PasswordHash).IsRequired();

            entity.Property(p => p.Role).HasConversion<string>().HasMaxLength(32);

            entity.HasIndex(p => p.Email).IsUnique();
            entity.HasIndex(p => p.EhicNumber).IsUnique();
        });
    }
}
