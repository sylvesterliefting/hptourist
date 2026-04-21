using Microsoft.EntityFrameworkCore;
using HPTourist.Models;

namespace HPTourist.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<PrescriptionRequest> PrescriptionRequests { get; set; } = default!;

    public DbSet<Prescription> Prescriptions { get; set; } = default!;
}
