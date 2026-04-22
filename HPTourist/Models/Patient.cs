namespace HPTourist.Models;

public class Patient : BaseEntity
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required DateOnly DateOfBirth { get; set; }

    public required string EhicNumber { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public UserRole Role { get; set; } = UserRole.Patient;
}
