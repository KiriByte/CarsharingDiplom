using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsharingProject.Migrations
{
    /// <inheritdoc />
    public partial class changeNameInRentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentHistory_RentCars_Vin",
                table: "RentHistory");

            migrationBuilder.RenameColumn(
                name: "Vin",
                table: "RentHistory",
                newName: "CarId");

            migrationBuilder.RenameIndex(
                name: "IX_RentHistory_Vin",
                table: "RentHistory",
                newName: "IX_RentHistory_CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentHistory_RentCars_CarId",
                table: "RentHistory",
                column: "CarId",
                principalTable: "RentCars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentHistory_RentCars_CarId",
                table: "RentHistory");

            migrationBuilder.RenameColumn(
                name: "CarId",
                table: "RentHistory",
                newName: "Vin");

            migrationBuilder.RenameIndex(
                name: "IX_RentHistory_CarId",
                table: "RentHistory",
                newName: "IX_RentHistory_Vin");

            migrationBuilder.AddForeignKey(
                name: "FK_RentHistory_RentCars_Vin",
                table: "RentHistory",
                column: "Vin",
                principalTable: "RentCars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
