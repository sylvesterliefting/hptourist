using System;
using System.Collections.Generic;
using System.Text;

namespace HPTourist.Data.Models
{
   public class Employee
   {
      public int Id { get; set; }
      public string Name { get; set; } = null!;
      public Role Role { get; set; }
      public int PracticeId { get; set; }
      public Practice Practice { get; set; } = null!;
   }
}
