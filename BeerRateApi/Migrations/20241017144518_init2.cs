using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeerRateApi.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beers_BeerImage_BeerImageId",
                table: "Beers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BeerImage",
                table: "BeerImage");

            migrationBuilder.RenameTable(
                name: "BeerImage",
                newName: "BeerImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BeerImages",
                table: "BeerImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Beers_BeerImages_BeerImageId",
                table: "Beers",
                column: "BeerImageId",
                principalTable: "BeerImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beers_BeerImages_BeerImageId",
                table: "Beers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BeerImages",
                table: "BeerImages");

            migrationBuilder.RenameTable(
                name: "BeerImages",
                newName: "BeerImage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BeerImage",
                table: "BeerImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Beers_BeerImage_BeerImageId",
                table: "Beers",
                column: "BeerImageId",
                principalTable: "BeerImage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
