using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedBlobStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureSource",
                table: "Book");

            migrationBuilder.AddColumn<string>(
                name: "PictureBlobName",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureBlobName",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Book");

            migrationBuilder.AddColumn<byte[]>(
                name: "PictureSource",
                table: "Book",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
