using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKhronus.DataAccess.Migrations.MyKhronus
{
    /// <inheritdoc />
    public partial class v_011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkItem_DayEntry_DayEntryEntryDate_DayEntryWorkItemId",
                schema: "mk",
                table: "WorkItem");

            migrationBuilder.DropIndex(
                name: "IX_WorkItem_DayEntryEntryDate_DayEntryWorkItemId",
                schema: "mk",
                table: "WorkItem");

            migrationBuilder.DropColumn(
                name: "DayEntryEntryDate",
                schema: "mk",
                table: "WorkItem");

            migrationBuilder.DropColumn(
                name: "DayEntryWorkItemId",
                schema: "mk",
                table: "WorkItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DayEntryEntryDate",
                schema: "mk",
                table: "WorkItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DayEntryWorkItemId",
                schema: "mk",
                table: "WorkItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkItem_DayEntryEntryDate_DayEntryWorkItemId",
                schema: "mk",
                table: "WorkItem",
                columns: new[] { "DayEntryEntryDate", "DayEntryWorkItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItem_DayEntry_DayEntryEntryDate_DayEntryWorkItemId",
                schema: "mk",
                table: "WorkItem",
                columns: new[] { "DayEntryEntryDate", "DayEntryWorkItemId" },
                principalSchema: "mk",
                principalTable: "DayEntry",
                principalColumns: new[] { "EntryDate", "WorkItemId" });
        }
    }
}
