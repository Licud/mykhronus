using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKhronus.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class added_initial_migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mk");

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
                name: "WorkItem",
                schema: "mk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItem_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "mk",
                        principalTable: "Project",
                        principalColumn: "Id");
                });

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
                name: "DayEntry",
                schema: "mk");

            migrationBuilder.DropTable(
                name: "WorkItem",
                schema: "mk");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "mk");
        }
    }
}
