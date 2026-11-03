using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vehicle_Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO ""VehicleStates"" (""Id"", ""Name"", ""CreatedAt"", ""UpdatedAt"")
VALUES
    (1, 'Active', NOW(), NOW()),
    (2, 'InActive', NOW(), NOW()),
    (3, 'Archived', NOW(), NOW())
ON CONFLICT (""Id"") DO NOTHING;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VehicleStates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VehicleStates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VehicleStates",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
