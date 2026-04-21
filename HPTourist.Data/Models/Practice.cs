using System;
using System.Collections.Generic;
using System.Text;

namespace HPTourist.Data.Models
{
   public class Practice
   {
      public int Id { get; set; }
      public string Name { get; set; } = null!;
      public string Address { get; set; } = null!;

      public ICollection<Employee> Employees { get; set; } = new List<Employee>();
      public ICollection<Patient> Patients { get; set; } = new List<Patient>();
   }
}
