using System.ComponentModel.DataAnnotations;

namespace HPTourist.Data.DTOs;

public class PatientLoginForm
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
