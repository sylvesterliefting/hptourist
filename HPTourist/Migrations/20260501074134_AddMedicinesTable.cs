using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPTourist.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicinesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF to_regclass('"Medicine"') IS NOT NULL AND to_regclass('"Medicines"') IS NULL THEN
                        ALTER TABLE "Medicine" DROP CONSTRAINT IF EXISTS "FK_Medicine_PrescriptionRequests_PrescriptionRequestId";
                        ALTER TABLE "Medicine" DROP CONSTRAINT IF EXISTS "FK_Medicine_Prescriptions_PrescriptionId";
                        ALTER TABLE "Medicine" DROP CONSTRAINT IF EXISTS "PK_Medicine";
                        ALTER TABLE "Medicine" RENAME TO "Medicines";
                        ALTER INDEX IF EXISTS "IX_Medicine_PrescriptionRequestId" RENAME TO "IX_Medicines_PrescriptionRequestId";
                        ALTER INDEX IF EXISTS "IX_Medicine_PrescriptionId" RENAME TO "IX_Medicines_PrescriptionId";
                        ALTER TABLE "Medicines" ADD CONSTRAINT "PK_Medicines" PRIMARY KEY ("Id");
                        ALTER TABLE "Medicines" ADD CONSTRAINT "FK_Medicines_PrescriptionRequests_PrescriptionRequestId" FOREIGN KEY ("PrescriptionRequestId") REFERENCES "PrescriptionRequests" ("Id");
                        ALTER TABLE "Medicines" ADD CONSTRAINT "FK_Medicines_Prescriptions_PrescriptionId" FOREIGN KEY ("PrescriptionId") REFERENCES "Prescriptions" ("Id");
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF to_regclass('"Medicines"') IS NOT NULL AND to_regclass('"Medicine"') IS NULL THEN
                        ALTER TABLE "Medicines" DROP CONSTRAINT IF EXISTS "FK_Medicines_PrescriptionRequests_PrescriptionRequestId";
                        ALTER TABLE "Medicines" DROP CONSTRAINT IF EXISTS "FK_Medicines_Prescriptions_PrescriptionId";
                        ALTER TABLE "Medicines" DROP CONSTRAINT IF EXISTS "PK_Medicines";
                        ALTER TABLE "Medicines" RENAME TO "Medicine";
                        ALTER INDEX IF EXISTS "IX_Medicines_PrescriptionRequestId" RENAME TO "IX_Medicine_PrescriptionRequestId";
                        ALTER INDEX IF EXISTS "IX_Medicines_PrescriptionId" RENAME TO "IX_Medicine_PrescriptionId";
                        ALTER TABLE "Medicine" ADD CONSTRAINT "PK_Medicine" PRIMARY KEY ("Id");
                        ALTER TABLE "Medicine" ADD CONSTRAINT "FK_Medicine_PrescriptionRequests_PrescriptionRequestId" FOREIGN KEY ("PrescriptionRequestId") REFERENCES "PrescriptionRequests" ("Id");
                        ALTER TABLE "Medicine" ADD CONSTRAINT "FK_Medicine_Prescriptions_PrescriptionId" FOREIGN KEY ("PrescriptionId") REFERENCES "Prescriptions" ("Id");
                    END IF;
                END $$;
                """);
        }
    }
}
