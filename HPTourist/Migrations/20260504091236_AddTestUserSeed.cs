using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class AddTestUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "DateOfBirth", "EHICId", "FirstName", "Gender", "LastName", "PracticeId", "PreferredLanguageId" },
                values: new object[] { new Guid("b2c3d4e5-2222-4222-8222-000000000002"), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Test", "Other", "User", new Guid("a1b2c3d4-1111-4111-8111-000000000001"), null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EmployeeId", "PasswordHash", "PatientId", "Role" },
                values: new object[] { new Guid("c3d4e5f6-3333-4333-8333-000000000003"), "test@chipsoft.com", null, "AQAAAAIAAYagAAAAEJ6u/hP+r4K+fK0XzL0mXpS9pXpS9pXpS9pXpS9pXpS9pXpS9pXpS9pXpS9pXpS9==", new Guid("b2c3d4e5-2222-4222-8222-000000000002"), "Patient" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-3333-4333-8333-000000000003"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-2222-4222-8222-000000000002"));
        }
    }
}
