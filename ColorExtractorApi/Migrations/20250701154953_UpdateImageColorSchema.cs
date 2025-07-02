using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColorExtractorApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageColorSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "ImageColors");

            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "ImageColors");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "ImageColors",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Thumbnail",
                table: "ImageColors",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "ImageColors");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "ImageColors");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "ImageColors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "ImageColors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
