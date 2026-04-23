using System.ComponentModel.DataAnnotations;
using HPTourist.Data.Models;

namespace HPTourist.Data.DTOs;

public class PatientInformationInputForm
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
    [AgeRange(0, 120)]
    [DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Please select a gender.")]
    [Display(Name = "Gender")]
    public Gender? Gender { get; set; }

    [Display(Name = "Allergies")]
    public List<AllergyInputForm> Allergies { get; set; } = [];

    [Display(Name = "Blood type")]
    public BloodType? BloodType { get; set; }

    [Display(Name = "Rh factor")]
    public RhFactor? RhFactor { get; set; }

    [Range(0.5, 500, ErrorMessage = "Weight must be between 0.5 and 500 kg.")]
    [Display(Name = "Weight (kg)")]
    public float? Weight { get; set; }
}
