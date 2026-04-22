using HPTourist.Models;
using Microsoft.EntityFrameworkCore;
using HPTourist.Data.Models;

namespace HPTourist.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
<<<<<<< HEAD
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
=======
   public DbSet<PrescriptionRequest> PrescriptionRequests => Set<PrescriptionRequest>();
   public DbSet<Prescription> Prescriptions => Set<Prescription>();
   public DbSet<Practice> Practices => Set<Practice>();
   public DbSet<Employee> Employees => Set<Employee>();
   public DbSet<Patient> Patients => Set<Patient>();
   public DbSet<Language> Languages => Set<Language>();
   public DbSet<Identification> Identificatios => Set<Identification>();
   public DbSet<EHIC> EHICs => Set<EHIC>();

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      //EHIC numbers are unique across the EU
      modelBuilder.Entity<EHIC>()
      .HasIndex(e => e.IdentificationId)
      .IsUnique();

      //Identification uniqueness is based on a single DocumentNumber per country
      modelBuilder.Entity<Identification>()
      .HasIndex(e => new { e.CountryCode, e.EncryptedDocumentNumber })
      .IsUnique();

        
      modelBuilder.Entity<Patient>(entity =>
      {
          entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
          entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
          entity.Property(p => p.Email).IsRequired().HasMaxLength(254);

          entity.HasIndex(p => p.Email).IsUnique();
      });

   }
>>>>>>> main
}
