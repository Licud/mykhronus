using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKhronus.DataAccess.Migrations.MyKhronus
{
    /// <inheritdoc />
    public partial class v_010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mk");

            migrationBuilder.CreateTable(
                name: "Activity",
                schema: "mk",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "DayEntry",
                schema: "mk",
                columns: table => new
                {
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayEntry", x => new { x.EntryDate, x.WorkItemId });
                });

            migrationBuilder.CreateTable(
                name: "Project",
                schema: "mk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityRecord",
                schema: "mk",
                columns: table => new
                {
                    ActivityRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRecord", x => x.ActivityRecordId);
                    table.ForeignKey(
                        name: "FK_ActivityRecord_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "mk",
                        principalTable: "Activity",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkItem",
                schema: "mk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    DayEntryEntryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DayEntryWorkItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItem_DayEntry_DayEntryEntryDate_DayEntryWorkItemId",
                        columns: x => new { x.DayEntryEntryDate, x.DayEntryWorkItemId },
                        principalSchema: "mk",
                        principalTable: "DayEntry",
                        principalColumns: new[] { "EntryDate", "WorkItemId" });
                    table.ForeignKey(
                        name: "FK_WorkItem_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "mk",
                        principalTable: "Project",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecord_ActivityId",
                schema: "mk",
                table: "ActivityRecord",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItem_DayEntryEntryDate_DayEntryWorkItemId",
                schema: "mk",
                table: "WorkItem",
                columns: new[] { "DayEntryEntryDate", "DayEntryWorkItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkItem_ProjectId",
                schema: "mk",
                table: "WorkItem",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityRecord",
                schema: "mk");

            migrationBuilder.DropTable(
                name: "WorkItem",
                schema: "mk");

            migrationBuilder.DropTable(
                name: "Activity",
                schema: "mk");

            migrationBuilder.DropTable(
                name: "DayEntry",
                schema: "mk");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "mk");
        }
    }
}
