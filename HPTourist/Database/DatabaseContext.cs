using Microsoft.EntityFrameworkCore;
using HPTourist.Data.Models;

namespace HPTourist.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
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

   }
}
