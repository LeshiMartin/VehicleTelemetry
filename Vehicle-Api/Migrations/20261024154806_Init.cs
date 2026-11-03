using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vehicle_Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Vin = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Make = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModelYear = table.Column<int>(type: "integer", nullable: false),
                    VehicleEngineType = table.Column<int>(type: "integer", nullable: false),
                    StateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StateValue = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleSignals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MinValue = table.Column<double>(type: "double precision", nullable: true),
                    MaxValue = table.Column<double>(type: "double precision", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    UnitName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnitSymbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleSignals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleSignals_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignalThresholds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleSignalId = table.Column<int>(type: "integer", nullable: false),
                    WarningMax = table.Column<double>(type: "double precision", nullable: true),
                    ErrorMin = table.Column<double>(type: "double precision", nullable: true),
                    ErrorMax = table.Column<double>(type: "double precision", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalThresholds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignalThresholds_VehicleSignals_VehicleSignalId",
                        column: x => x.VehicleSignalId,
                        principalTable: "VehicleSignals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignalThresholds_Id",
                table: "SignalThresholds",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignalThresholds_VehicleSignalId",
                table: "SignalThresholds",
                column: "VehicleSignalId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Id",
                table: "Vehicles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleSignals_Id",
                table: "VehicleSignals",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleSignals_VehicleId",
                table: "VehicleSignals",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignalThresholds");

            migrationBuilder.DropTable(
                name: "VehicleSignals");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
