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
         migrationBuilder.Sql($@"CREATE FUNCTION prevent_delete() RETURNS trigger AS $$
                                    BEGIN
                                      RAISE EXCEPTION '{Exceptions.WGBO_Database_Table_Deletes_Not_Allowed}';
                                    END;
                                    $$ LANGUAGE plpgsql;");

         migrationBuilder.Sql(string.Format(@"CREATE TRIGGER {0}
                                    BEFORE DELETE ON ""Practices""
                                    FOR EACH ROW EXECUTE FUNCTION prevent_delete();", FunctionNames.PRACTICE_CANT_BE_DELETED));

         migrationBuilder.Sql(string.Format(@"CREATE TRIGGER {0}
                                    BEFORE DELETE ON ""Employees""
                                    FOR EACH ROW EXECUTE FUNCTION prevent_delete();", FunctionNames.EMPLOYEE_CANT_BE_DELETED));


         migrationBuilder.Sql(string.Format(@"CREATE TRIGGER {0}
                                    BEFORE DELETE ON ""PrescriptionRequests""
                                    FOR EACH ROW EXECUTE FUNCTION prevent_delete();", FunctionNames.PRESCRIPTIONREQUEST_CANT_BE_DELETED));

         migrationBuilder.Sql(string.Format(@"CREATE TRIGGER {0}
                                    BEFORE DELETE ON ""EHICs""
                                    FOR EACH ROW EXECUTE FUNCTION prevent_delete();", FunctionNames.EHIC_CANT_BE_DELETED));

         migrationBuilder.Sql(string.Format(@"CREATE TRIGGER {0}
                                    BEFORE DELETE ON ""Identificatios""
                                    FOR EACH ROW EXECUTE FUNCTION prevent_delete();", FunctionNames.IDENTIFICATION_CANT_BE_DELETED));

         migrationBuilder.Sql(string.Format(@"CREATE TRIGGER {0}
                                    BEFORE DELETE ON ""Prescriptions""
                                    FOR EACH ROW EXECUTE FUNCTION prevent_delete();", FunctionNames.PRESCRIPTION_CANT_BE_DELETED));

         migrationBuilder.Sql(string.Format(@"CREATE TRIGGER {0}
                                    BEFORE DELETE ON ""Patients""
                                    FOR EACH ROW EXECUTE FUNCTION prevent_delete();", FunctionNames.PATIENT_CANT_BE_DELETED));
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
         //Deliberately kept blank. Unimportant for a demo application.
      }
   }
}
