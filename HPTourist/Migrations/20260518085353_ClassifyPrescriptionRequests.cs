using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class ClassifyPrescriptionRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "PrescriptionRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE "PrescriptionRequests" AS request
                SET "Type" = 1
                WHERE EXISTS (
                    SELECT 1
                    FROM "Medicines" AS medicine
                    WHERE medicine."PrescriptionRequestId" = request."Id"
                      AND medicine."ActiveSubstance" = 'Onbekend'
                      AND medicine."PharmaceuticalForm" = 'Onbekend'
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "PrescriptionRequests");
        }
    }
}
