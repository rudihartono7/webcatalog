using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAIFWebCatalog.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Companies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CategoryId",
                table: "Companies",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Categories_CategoryId",
                table: "Companies",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Categories_CategoryId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CategoryId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Companies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
