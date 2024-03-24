using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.LeaveManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class renamednoOfDaysColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumerOfDays",
                table: "LeaveAllocations",
                newName: "NumberOfDays");

            migrationBuilder.UpdateData(
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2024, 3, 24, 9, 48, 39, 496, DateTimeKind.Local).AddTicks(1319), new DateTime(2024, 3, 24, 9, 48, 39, 496, DateTimeKind.Local).AddTicks(1329) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfDays",
                table: "LeaveAllocations",
                newName: "NumerOfDays");

            migrationBuilder.UpdateData(
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "DateModified" },
                values: new object[] { new DateTime(2024, 3, 2, 18, 41, 50, 152, DateTimeKind.Local).AddTicks(9256), new DateTime(2024, 3, 2, 18, 41, 50, 152, DateTimeKind.Local).AddTicks(9269) });
        }
    }
}
