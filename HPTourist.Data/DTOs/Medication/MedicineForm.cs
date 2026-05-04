using System.ComponentModel.DataAnnotations;

namespace HPTourist.Data.DTOs.Medication;

public class MedicineForm
{
    [Required(ErrorMessage = "Medicatienaam is verplicht.")]
    [StringLength(100, ErrorMessage = "Medicatienaam mag maximaal 100 tekens zijn.")]
    public string Name { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9]{7}$", ErrorMessage = "Voer een geldige ATC-code in (7 karakters).")]
    public string? AtcCode { get; set; }

    [Required(ErrorMessage = "Werkzame stof is verplicht.")]
    [StringLength(100, ErrorMessage = "Werkzame stof mag maximaal 100 tekens zijn.")]
    public string ActiveSubstance { get; set; } = string.Empty;

    [Required(ErrorMessage = "Farmaceutische vorm is verplicht.")]
    [StringLength(50, ErrorMessage = "Farmaceutische vorm mag maximaal 50 tekens zijn.")]
    public string PharmaceuticalForm { get; set; } = string.Empty;
}
