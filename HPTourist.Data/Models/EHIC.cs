namespace HPTourist.Data.Models;

public class EHIC
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string EncryptedEHICNumber { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public Guid? IdentificationId { get; set; }
    public Identification? Identification { get; set; }
}
