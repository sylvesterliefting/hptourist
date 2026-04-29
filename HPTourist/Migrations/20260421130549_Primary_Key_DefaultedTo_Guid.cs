using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class Primary_Key_DefaultedTo_Guid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
      {
         //Drop identity on ID columns
         migrationBuilder.Sql(@"ALTER TABLE ""Practices"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
         migrationBuilder.Sql(@"ALTER TABLE ""Patients"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
         migrationBuilder.Sql(@"ALTER TABLE ""Languages"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
         migrationBuilder.Sql(@"ALTER TABLE ""Identificatios"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
         migrationBuilder.Sql(@"ALTER TABLE ""Employees"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
         migrationBuilder.Sql(@"ALTER TABLE ""EHICs"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");


         //Drop FK constraints

         migrationBuilder.DropForeignKey(
             name: "FK_Patients_EHICs_EHICId",
             table: "Patients");

         migrationBuilder.DropForeignKey(
             name: "FK_Patients_Languages_PreferredLanguageId",
             table: "Patients");

         migrationBuilder.DropForeignKey(
             name: "FK_Patients_Practices_PracticeId",
             table: "Patients");

         migrationBuilder.DropForeignKey(
             name: "FK_Identificatios_Patients_PatientId",
             table: "Identificatios");

         migrationBuilder.DropForeignKey(
             name: "FK_EHICs_Identificatios_IdentificationId",
             table: "EHICs");

         migrationBuilder.DropForeignKey(
             name: "FK_Employees_Practices_PracticeId",
             table: "Employees");



         migrationBuilder.DropColumn(table: "Practices", name: "Id");
         migrationBuilder.AddColumn<Guid>(table: "Practices", name: "Id", type: "uuid", nullable: false);


         migrationBuilder.AddPrimaryKey(name: "PK_Practices", table: "Practices", column: "Id");

         migrationBuilder.DropColumn(table: "Patients", name: "PreferredLanguageId");
         migrationBuilder.AddColumn<Guid>(table: "Patients", name: "PreferredLanguageId", type: "uuid", nullable: false);



         migrationBuilder.DropColumn(table: "Patients", name: "PracticeId");
         migrationBuilder.AddColumn<Guid>(table: "Patients", name: "PracticeId", type: "uuid", nullable: true);


         migrationBuilder.DropColumn(table: "Patients", name: "EHICId");
         migrationBuilder.AddColumn<Guid>(table: "Patients", name: "EHICId", type: "uuid", nullable: true);

         migrationBuilder.DropColumn(table: "Patients", name: "Id");
         migrationBuilder.AddColumn<Guid>(table: "Patients", name: "Id", type: "uuid", nullable: false);


         migrationBuilder.AddPrimaryKey(name: "PK_Patients", table: "Patients", column: "Id");

         migrationBuilder.DropColumn(table: "Languages", name: "Id");
         migrationBuilder.AddColumn<Guid>(table: "Languages", name: "Id", type: "uuid", nullable: false);

         migrationBuilder.AddPrimaryKey(name: "PK_Languages", table: "Languages", column: "Id");

         migrationBuilder.DropColumn(table: "Identificatios", name: "PatientId");
         migrationBuilder.AddColumn<Guid>(table: "Identificatios", name: "PatientId", type: "uuid", nullable: false);


         migrationBuilder.DropColumn(table: "Identificatios", name: "Id");
         migrationBuilder.AddColumn<Guid>(table: "Identificatios", name: "Id", type: "uuid", nullable: false);

         migrationBuilder.AddPrimaryKey(name: "PK_Identifications", table: "Identificatios", column: "Id");

         migrationBuilder.DropColumn(table: "Employees", name: "PracticeId");
         migrationBuilder.AddColumn<Guid>(table: "Employees", name: "PracticeId", type: "uuid", nullable: false);


         migrationBuilder.DropColumn(table: "Employees", name: "Id");
         migrationBuilder.AddColumn<Guid>(table: "Employees", name: "Id", type: "uuid", nullable: false);

         migrationBuilder.AddPrimaryKey(name: "PK_Employees", table: "Employees", column: "Id");

         migrationBuilder.DropColumn(table: "EHICs", name: "IdentificationId");
         migrationBuilder.AddColumn<Guid>(table: "EHICs", name: "IdentificationId", type: "uuid", nullable: false);

         migrationBuilder.DropColumn(table: "EHICs", name: "Id");
         migrationBuilder.AddColumn<Guid>(table: "EHICs", name: "Id", type: "uuid", nullable: false);

         migrationBuilder.AddPrimaryKey(name: "PK_EHICs", table: "EHICs", column: "Id");

         //Recreate FK constraints
         migrationBuilder.AddForeignKey(
            name: "FK_Patients_EHICs_EHICId",
            table: "Patients",
            column: "EHICId",
            principalTable: "EHICs",
            principalColumn: "Id");
         migrationBuilder.AddForeignKey(
            name: "FK_Patients_Languages_PreferredLanguageId",
            table: "Patients",
            column: "PreferredLanguageId",
            principalTable: "Languages",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
         migrationBuilder.AddForeignKey(
             name: "FK_Patients_Practices_PracticeId",
             table: "Patients",
             column: "PracticeId",
             principalTable: "Practices",
             principalColumn: "Id",
             onDelete: ReferentialAction.Cascade);
         migrationBuilder.AddForeignKey(
             name: "FK_Identificatios_Patients_PatientId",
             table: "Identificatios",
             column: "PatientId",
             principalTable: "Practices",
             principalColumn: "Id",
             onDelete: ReferentialAction.Cascade);
         migrationBuilder.AddForeignKey(
             name: "FK_EHICs_Identificatios_IdentificationId",
             table: "EHICs",
             column: "IdentificationId",
             principalTable: "Identificatios",
             principalColumn: "Id",
             onDelete: ReferentialAction.Cascade);
         migrationBuilder.AddForeignKey(
             name: "FK_Employees_Practices_PracticeId",
             table: "Employees",
             column: "PracticeId",
             principalTable: "Practices",
             principalColumn: "Id",
             onDelete: ReferentialAction.Cascade);

      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
         //Deliberately kept blank. Unimportant for a demo application.
      }
   }
}
