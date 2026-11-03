using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Api.Migrations
{
    /// <inheritdoc />
    public partial class Fix_VehicleState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleState_VehicleStateId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleState",
                table: "VehicleState");

            migrationBuilder.RenameTable(
                name: "VehicleState",
                newName: "VehicleStates");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleState_Name",
                table: "VehicleStates",
                newName: "IX_VehicleStates_Name");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleState_Id",
                table: "VehicleStates",
                newName: "IX_VehicleStates_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleStates",
                table: "VehicleStates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleStates_VehicleStateId",
                table: "Vehicles",
                column: "VehicleStateId",
                principalTable: "VehicleStates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleStates_VehicleStateId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleStates",
                table: "VehicleStates");

            migrationBuilder.RenameTable(
                name: "VehicleStates",
                newName: "VehicleState");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleStates_Name",
                table: "VehicleState",
                newName: "IX_VehicleState_Name");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleStates_Id",
                table: "VehicleState",
                newName: "IX_VehicleState_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleState",
                table: "VehicleState",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleState_VehicleStateId",
                table: "Vehicles",
                column: "VehicleStateId",
                principalTable: "VehicleState",
                principalColumn: "Id");
        }
    }
}
