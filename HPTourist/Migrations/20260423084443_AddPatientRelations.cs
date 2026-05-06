using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicine_PrescriptionRequests_PrescriptionRequestId",
                table: "Medicine");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicine_Prescriptions_PrescriptionId",
                table: "Medicine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medicine",
                table: "Medicine");

            migrationBuilder.RenameTable(
                name: "Medicine",
                newName: "Medicines");

            migrationBuilder.RenameIndex(
                name: "IX_Medicine_PrescriptionRequestId",
                table: "Medicines",
                newName: "IX_Medicines_PrescriptionRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Medicine_PrescriptionId",
                table: "Medicines",
                newName: "IX_Medicines_PrescriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medicines",
                table: "Medicines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_PrescriptionRequests_PrescriptionRequestId",
                table: "Medicines",
                column: "PrescriptionRequestId",
                principalTable: "PrescriptionRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Prescriptions_PrescriptionId",
                table: "Medicines",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_PrescriptionRequests_PrescriptionRequestId",
                table: "Medicines");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Prescriptions_PrescriptionId",
                table: "Medicines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medicines",
                table: "Medicines");

            migrationBuilder.RenameTable(
                name: "Medicines",
                newName: "Medicine");

            migrationBuilder.RenameIndex(
                name: "IX_Medicines_PrescriptionRequestId",
                table: "Medicine",
                newName: "IX_Medicine_PrescriptionRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Medicines_PrescriptionId",
                table: "Medicine",
                newName: "IX_Medicine_PrescriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medicine",
                table: "Medicine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicine_PrescriptionRequests_PrescriptionRequestId",
                table: "Medicine",
                column: "PrescriptionRequestId",
                principalTable: "PrescriptionRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicine_Prescriptions_PrescriptionId",
                table: "Medicine",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }
    }
}
