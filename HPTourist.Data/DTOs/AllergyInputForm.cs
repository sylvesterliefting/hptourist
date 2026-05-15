using System.ComponentModel.DataAnnotations;

namespace HPTourist.Data.DTOs;

public class AllergyInputForm
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Substance")]
    public string Substance { get; set; } = string.Empty;

    [StringLength(250)]
    [Display(Name = "Reaction")]
    public string? Reaction { get; set; }
}

