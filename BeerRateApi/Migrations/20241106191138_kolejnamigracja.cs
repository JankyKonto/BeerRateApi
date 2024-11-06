using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeerRateApi.Migrations
{
    /// <inheritdoc />
    public partial class kolejnamigracja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isComitted",
                table: "Beers",
                newName: "isConfirmed");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "BeerImages",
                newName: "ContentType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isConfirmed",
                table: "Beers",
                newName: "isComitted");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "BeerImages",
                newName: "FileName");
        }
    }
}
