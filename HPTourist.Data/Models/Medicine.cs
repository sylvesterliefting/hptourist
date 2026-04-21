namespace HPTourist.Data.Models;

public class Medicine
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = default!;
    public string AtcCode { get; set; } = default!;
    public string ActiveSubstance { get; set; } = default!;
    public string PharmaceuticalForm { get; set; } = default!;
    
}
