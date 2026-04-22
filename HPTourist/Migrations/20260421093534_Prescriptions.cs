using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class Prescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrescriptionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestStatus = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescriptionRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medicine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AtcCode = table.Column<string>(type: "text", nullable: false),
                    ActiveSubstance = table.Column<string>(type: "text", nullable: false),
                    PharmaceuticalForm = table.Column<string>(type: "text", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PrescriptionRequestId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicine_PrescriptionRequests_PrescriptionRequestId",
                        column: x => x.PrescriptionRequestId,
                        principalTable: "PrescriptionRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Medicine_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_PrescriptionId",
                table: "Medicine",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_PrescriptionRequestId",
                table: "Medicine",
                column: "PrescriptionRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicine");

            migrationBuilder.DropTable(
                name: "PrescriptionRequests");

            migrationBuilder.DropTable(
                name: "Prescriptions");
        }
    }
}
