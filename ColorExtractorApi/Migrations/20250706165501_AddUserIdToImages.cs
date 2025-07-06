using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColorExtractorApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ImageColors",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_ImageColors_UserId",
                table: "ImageColors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageColors_Users_UserId",
                table: "ImageColors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageColors_Users_UserId",
                table: "ImageColors");

            migrationBuilder.DropIndex(
                name: "IX_ImageColors_UserId",
                table: "ImageColors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ImageColors");
        }
    }
}
