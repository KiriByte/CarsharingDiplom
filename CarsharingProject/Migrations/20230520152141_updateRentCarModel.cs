using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsharingProject.Migrations
{
    /// <inheritdoc />
    public partial class updateRentCarModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PricePerMinute",
                table: "RentCars",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerMinute",
                table: "RentCars");
        }
    }
}
