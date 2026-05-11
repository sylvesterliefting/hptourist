namespace HPTourist.Data.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? PatientId { get; set; }
    public Patient? Patient { get; set; }

    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
