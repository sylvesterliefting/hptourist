using Microsoft.EntityFrameworkCore.Migrations;
using HPTourist.Data;
#nullable disable

namespace HPTourist.Migrations
{
   /// <inheritdoc />
   public partial class RetentionPolicy_init : Migration
   {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
         //it should be impossible to remove certain types of data due to WGBO.
         //instead, deletes should enforce anonimization and/or status flags due comply to GDPR/AVG.
         migrationBuilder.Sql($@"CREATE OR REPLACE FUNCTION prevent_delete() RETURNS trigger AS $$
                                    BEGIN
                                      RAISE EXCEPTION '{Exceptions.WGBO_Database_Table_Deletes_Not_Allowed}';
                                    END;
                                    $$ LANGUAGE plpgsql;");

         CreatePreventDeleteTrigger(migrationBuilder, "Practices", FunctionNames.PRACTICE_CANT_BE_DELETED);
         CreatePreventDeleteTrigger(migrationBuilder, "Employees", FunctionNames.EMPLOYEE_CANT_BE_DELETED);
         CreatePreventDeleteTrigger(migrationBuilder, "PrescriptionRequests", FunctionNames.PRESCRIPTIONREQUEST_CANT_BE_DELETED);
         CreatePreventDeleteTrigger(migrationBuilder, "EHICs", FunctionNames.EHIC_CANT_BE_DELETED);
         CreatePreventDeleteTrigger(migrationBuilder, "Identificatios", FunctionNames.IDENTIFICATION_CANT_BE_DELETED);
         CreatePreventDeleteTrigger(migrationBuilder, "Prescriptions", FunctionNames.PRESCRIPTION_CANT_BE_DELETED);
         CreatePreventDeleteTrigger(migrationBuilder, "Patients", FunctionNames.PATIENT_CANT_BE_DELETED);
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
         //Deliberately kept blank. Unimportant for a demo application.
      }

      private static void CreatePreventDeleteTrigger(MigrationBuilder migrationBuilder, string tableName, string triggerName)
      {
         migrationBuilder.Sql($@"DROP TRIGGER IF EXISTS ""{triggerName}"" ON ""{tableName}"";
                                CREATE TRIGGER ""{triggerName}""
                                BEFORE DELETE ON ""{tableName}""
                                FOR EACH ROW EXECUTE FUNCTION prevent_delete();");
      }
   }
}
