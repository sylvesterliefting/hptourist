namespace HPTourist.Models;

public class Medicine
{
    public Guid Id { get; set; } 
    required public string Name { get; set; } 
    required public string AtcCode { get; set; }
    required public string ActiveSubstance { get; set; }
    required public string PharmaceuticalForm { get; set; }
    
}
