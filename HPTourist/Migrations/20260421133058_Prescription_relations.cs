using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class Prescription_relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Prescriptions",
                newName: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_EmployeeId",
                table: "Prescriptions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PatientId",
                table: "Prescriptions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PrescriptionRequestId",
                table: "Prescriptions",
                column: "PrescriptionRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionRequests_PatientId",
                table: "PrescriptionRequests",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionRequests_Patients_PatientId",
                table: "PrescriptionRequests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Employees_EmployeeId",
                table: "Prescriptions",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Patients_PatientId",
                table: "Prescriptions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_PrescriptionRequests_PrescriptionRequestId",
                table: "Prescriptions",
                column: "PrescriptionRequestId",
                principalTable: "PrescriptionRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionRequests_Patients_PatientId",
                table: "PrescriptionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Employees_EmployeeId",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Patients_PatientId",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_PrescriptionRequests_PrescriptionRequestId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_EmployeeId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_PatientId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_PrescriptionRequestId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionRequests_PatientId",
                table: "PrescriptionRequests");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Prescriptions",
                newName: "DoctorId");
        }
    }
}
