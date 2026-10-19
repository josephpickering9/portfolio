using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseStudyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyDecision",
                table: "Project",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                table: "Project",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProblemStatement",
                table: "Project",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectStat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStat_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStat_ProjectId",
                table: "ProjectStat",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectStat");

            migrationBuilder.DropColumn(
                name: "KeyDecision",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProblemStatement",
                table: "Project");
        }
    }
}
