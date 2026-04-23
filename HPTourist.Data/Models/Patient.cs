namespace HPTourist.Data.Models;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public Guid PracticeId { get; set; }
    public Practice Practice { get; set; } = null!;

    public Guid? PreferredLanguageId { get; set; }
    public Language? PreferredLanguage { get; set; }

    public Identification? Identification { get; set; }

    public EHIC? EHIC { get; set; }
}
