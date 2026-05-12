using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class PatientMedicalFieldsAndAllergyDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "Patients",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RhFactor",
                table: "Patients",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "Patients",
                type: "real",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Allergies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Substance = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Reaction = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Allergies_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allergies_PatientId",
                table: "Allergies",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allergies");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RhFactor",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Patients");
        }
    }
}
