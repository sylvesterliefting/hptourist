using System;
using System.Collections.Generic;
using System.Text;

namespace HPTourist.Data.Models
{
   public class Identification
   {
      public int Id { get; set; }
      public int PatientId { get; set; }
      public Patient Patient { get; set; } = null!;
      public string CountryCode { get; set; } = null!;
      public string DocumentType { get; set; } = null!;
      public string EncryptedDocumentNumber { get; set; } = null!;
      public string EncryptedIdentificationNumber { get; set; } = null!;
      public DateTime ValidFrom { get; set; }
      public DateTime? ValidTo { get; set; }
      public EHIC? EHIC { get; set; }
   }
}

