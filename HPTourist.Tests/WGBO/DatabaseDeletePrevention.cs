using HPTourist.Data.Models;
using HPTourist.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HPTourist.Tests.WGBO
{
   /// <summary>
   /// Part of WGBO compliance.
   /// Make sure the database protects itself from unwarranted deletion of records.
   /// Instead, records should be anonimized to comply to AVG.
   /// This allows an application-independent approach to enforce and protect the data.
   /// </summary>
   [TestClass]
   public sealed class DatabaseDeletePrevention
   {
      WebApplication app;

      public DatabaseDeletePrevention(){
         app = Program.Setup(new string[]{ "Test" });
      }

      [TestInitialize]
      public void TestInit()
      {
         // This method is called before each test method.
         using (var scope = app.Services.CreateScope())
         {
            var db = scope.ServiceProvider.GetService<DatabaseContext>();
            db.ChangeTracker.Clear();
         }
      }

      [TestMethod]
      public void PreventDeletePatient()
      {
         using (var scope = app.Services.CreateScope()){
            var firstName = "preventDelete";
            var lastName = "lastname";
            var email = "email@example.com";
            
            //set up
            var db = scope.ServiceProvider.GetService<DatabaseContext>();
            {
               db.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Patients\" RESTART IDENTITY CASCADE;");
               db.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE;");
            }

            var patient = new Patient() { FirstName = firstName, LastName = lastName, Gender = Gender.Unknown };
            patient.Practice = db.Practices.First();
            var user = new User() { Email = email, Patient = patient, PasswordHash = "fakeHash" };
            db.Add<User>(user);
            db.SaveChanges();

            var patientToDelete = db.Patients.Single(x => x.FirstName == firstName);
            //WARNING: deleting users atm is still a valid operation.
            db.Users.Remove(db.Users.Single(x => x.Email == email));
            db.SaveChanges();

            //Try and remove the patient - should throw an exception due to WGBO rules.
            db.Patients.Remove(patientToDelete);
            Assert.ThrowsExactly<DbUpdateException>(() => db.SaveChanges(), Exceptions.WGBO_Database_Table_Deletes_Not_Allowed);
            

            db.ChangeTracker.Clear();
            var patientsInDb = db.Patients.Count();
            Assert.AreEqual(1, patientsInDb, "Expecting a single patient to still be in the database, instead, a different number of Patients was found.");
         }
      }
   }
}
