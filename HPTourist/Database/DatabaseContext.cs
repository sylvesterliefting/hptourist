using Microsoft.EntityFrameworkCore;

namespace HPTourist.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
}
