namespace HPTourist.Data.Models;

public class Allergy {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Substance { get; set; } = null!;
    public string? Reaction { get; set; }
}