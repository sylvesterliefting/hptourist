using HPTourist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Practice> Practices => Set<Practice>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Identification> Identificatios => Set<Identification>();
    public DbSet<EHIC> EHICs => Set<EHIC>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionRequest> PrescriptionRequests => Set<PrescriptionRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Email).IsRequired().HasMaxLength(254);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(32);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.Patient)
                  .WithOne()
                  .HasForeignKey<User>(u => u.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.Employee)
                  .WithOne()
                  .HasForeignKey<User>(u => u.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Users_OneOfPatientOrEmployee",
                "(\"PatientId\" IS NOT NULL) <> (\"EmployeeId\" IS NOT NULL)"));
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Gender).HasConversion<string>().HasMaxLength(16);
        });

        // EHIC numbers are unique across the EU.
        modelBuilder.Entity<EHIC>()
                    .HasIndex(e => e.EncryptedEHICNumber)
                    .IsUnique();

        modelBuilder.Entity<Identification>(entity =>
        {
            entity.HasKey(i => i.Id).HasName("PK_Identifications");
            entity.HasIndex(e => new { e.CountryCode, e.EncryptedDocumentNumber }).IsUnique();
        });

        modelBuilder.Entity<Practice>().HasData(new Practice
        {
            Id = SeededIds.TouristDoctorAmsterdamPractice,
            Name = "Huisartsenpraktijk Tourist Doctor Amsterdam",
            Address = "Damrak 1, 1012 LG Amsterdam",
        });
    }
}
