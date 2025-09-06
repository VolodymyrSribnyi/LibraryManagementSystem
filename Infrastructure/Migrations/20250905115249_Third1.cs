using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Third1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "LibraryCard",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "DATEADD(YEAR, 1, GETUTCDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2026, 9, 5, 11, 49, 31, 286, DateTimeKind.Utc).AddTicks(9498));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "LibraryCard",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 9, 5, 11, 49, 31, 286, DateTimeKind.Utc).AddTicks(9498),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "DATEADD(YEAR, 1, GETUTCDATE())");
        }
    }
}
