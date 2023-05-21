using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsharingProject.Migrations
{
    /// <inheritdoc />
    public partial class rentModelAddOdometer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RentOdometerEnd",
                table: "RentHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentOdometerStart",
                table: "RentHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentOdometerEnd",
                table: "RentHistory");

            migrationBuilder.DropColumn(
                name: "RentOdometerStart",
                table: "RentHistory");
        }
    }
}
