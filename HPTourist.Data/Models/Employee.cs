using System;
using System.Collections.Generic;
using System.Text;

namespace HPTourist.Data.Models
{
   public class Employee
   {
      public Guid Id { get; set; }
      public string Name { get; set; } = null!;
      public Role Role { get; set; }
      public Guid PracticeId { get; set; }
      public Practice Practice { get; set; } = null!;
   }
}
