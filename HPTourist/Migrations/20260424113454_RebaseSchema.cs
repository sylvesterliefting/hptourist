using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
   /// <inheritdoc />
   public partial class RebaseSchema : Migration
   {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
         migrationBuilder.DropColumn(
             name: "Role",
             table: "Users");

         migrationBuilder.DropColumn(
             name: "Gender",
             table: "Patients");

         migrationBuilder.AddColumn<int>(
             name: "Role",
             table: "Users",
             type: "integer",
             nullable: false);

         migrationBuilder.AddColumn<int>(
             name: "Gender",
             table: "Patients",
             type: "integer",
             nullable: false);
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
         migrationBuilder.AlterColumn<string>(
             name: "Role",
             table: "Users",
             type: "character varying(32)",
             maxLength: 32,
             nullable: false,
             oldClrType: typeof(int),
             oldType: "integer");

         migrationBuilder.AlterColumn<string>(
             name: "Gender",
             table: "Patients",
             type: "character varying(16)",
             maxLength: 16,
             nullable: false,
             oldClrType: typeof(int),
             oldType: "integer");

         migrationBuilder.AlterColumn<string>(
             name: "Gender",
             table: "Patients",
             type: "character varying(16)",
             maxLength: 16,
             nullable: false,
             oldClrType: typeof(int),
             oldType: "integer");

         migrationBuilder.AlterColumn<string>(
             name: "Role",
             table: "Users",
             type: "character varying(16)",
             maxLength: 32,
             nullable: false,
             oldClrType: typeof(int),
             oldType: "integer");
      }
   }
}
