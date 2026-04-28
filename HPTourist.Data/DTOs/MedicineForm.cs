using System.ComponentModel.DataAnnotations;

namespace HPTourist.Data.DTOs;

public sealed class MedicineForm
{
    [Required(ErrorMessage = "Medicatienaam is verplicht.")]
    [StringLength(100, ErrorMessage = "Medicatienaam mag maximaal 100 tekens zijn.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "ATC-code mag maximaal 20 tekens zijn.")]
    public string? AtcCode { get; set; }

    [Required(ErrorMessage = "Werkzame stof is verplicht.")]
    [StringLength(100, ErrorMessage = "Werkzame stof mag maximaal 100 tekens zijn.")]
    public string ActiveSubstance { get; set; } = string.Empty;

    [Required(ErrorMessage = "Farmaceutische vorm is verplicht.")]
    [StringLength(50, ErrorMessage = "Farmaceutische vorm mag maximaal 50 tekens zijn.")]
    public string PharmaceuticalForm { get; set; } = string.Empty;
}
