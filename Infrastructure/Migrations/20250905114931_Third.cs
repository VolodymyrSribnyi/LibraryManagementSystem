using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservedAt",
                table: "Reservation",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 9, 5, 11, 37, 52, 772, DateTimeKind.Utc).AddTicks(3286));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndsAt",
                table: "Reservation",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "DATEADD(DAY, 14, GETUTCDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 9, 19, 11, 37, 52, 772, DateTimeKind.Utc).AddTicks(3629));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "LibraryCard",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 9, 5, 11, 49, 31, 286, DateTimeKind.Utc).AddTicks(9498),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2026, 9, 5, 11, 37, 52, 770, DateTimeKind.Utc).AddTicks(7823));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservedAt",
                table: "Reservation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 5, 11, 37, 52, 772, DateTimeKind.Utc).AddTicks(3286),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndsAt",
                table: "Reservation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 11, 37, 52, 772, DateTimeKind.Utc).AddTicks(3629),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "DATEADD(DAY, 14, GETUTCDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "LibraryCard",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 9, 5, 11, 37, 52, 770, DateTimeKind.Utc).AddTicks(7823),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2026, 9, 5, 11, 49, 31, 286, DateTimeKind.Utc).AddTicks(9498));
        }
    }
}
