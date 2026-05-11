using System.ComponentModel.DataAnnotations;
using HPTourist.Data.Models;

namespace HPTourist.Data.DTOs;

public class PatientRegistrationForm
{
    [Required]
    [StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [AgeRange(0, 100)]
    [DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Please select a gender.")]
    public Gender? Gender { get; set; }

    [Required]
    [RegularExpression("^[A-Za-z0-9]{20}$", ErrorMessage = "EHIC number must be exactly 20 alphanumeric characters.")]
    [Display(Name = "EHIC number")]
    public string EhicNumber { get; set; } = string.Empty;

    [Required]
    [FutureDate]
    [DataType(DataType.Date)]
    [Display(Name = "EHIC expiry date")]
    public DateTime? EhicExpiryDate { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string PasswordConfirm { get; set; } = string.Empty;
}
