using System;
using System.Collections.Generic;
using System.Text;

namespace HPTourist.Data.Models
{
   public class EHIC
   {
      public int Id { get; set; }

      public string EncryptedEHICNumber { get; set; } = null!;

      public DateTime ExpiryDate { get; set; }

      public int IdentificationId { get; set; }
      public Identification Identification { get; set; } = null!;
   }
}
