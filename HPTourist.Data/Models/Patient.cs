using System;
using System.Collections.Generic;
using System.Text;

namespace HPTourist.Data.Models
{
   public class Patient
   {
      public int Id { get; set; }
      public string FirstName { get; set; } = null!;
      public string LastName { get; set; } = null!;
      public string Email { get; set; } = null!;
      public DateTime DateOfBirth { get; set; }
      public Gender Gender { get; set; }
      public int PracticeId { get; set; }
      public Practice Practice { get; set; } = null!;
      public int PreferredLanguageId { get; set; }
      public Language PreferredLanguage { get; set; } = null!;
      public Identification? Identification { get; set; }
      public EHIC? EHIC { get; set; }
   }
}
