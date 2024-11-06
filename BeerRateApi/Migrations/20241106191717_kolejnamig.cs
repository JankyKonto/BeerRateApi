using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeerRateApi.Migrations
{
    /// <inheritdoc />
    public partial class kolejnamig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isConfirmed",
                table: "Beers",
                newName: "IsConfirmed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsConfirmed",
                table: "Beers",
                newName: "isConfirmed");
        }
    }
}
