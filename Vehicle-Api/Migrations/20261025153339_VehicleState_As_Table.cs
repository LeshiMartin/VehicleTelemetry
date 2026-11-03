using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vehicle_Api.Migrations
{
    /// <inheritdoc />
    public partial class VehicleState_As_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateName",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "StateValue",
                table: "Vehicles");

            migrationBuilder.AddColumn<int>(
                name: "VehicleStateId",
                table: "Vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VehicleState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleState", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleStateId",
                table: "Vehicles",
                column: "VehicleStateId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleState_Id",
                table: "VehicleState",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleState_Name",
                table: "VehicleState",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleState_VehicleStateId",
                table: "Vehicles",
                column: "VehicleStateId",
                principalTable: "VehicleState",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleState_VehicleStateId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleState");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleStateId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleStateId",
                table: "Vehicles");

            migrationBuilder.AddColumn<string>(
                name: "StateName",
                table: "Vehicles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StateValue",
                table: "Vehicles",
                type: "integer",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);
        }
    }
}
