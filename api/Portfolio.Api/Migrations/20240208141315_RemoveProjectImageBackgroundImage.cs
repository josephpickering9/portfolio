using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProjectImageBackgroundImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "Project",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Project",
                type: "text",
                nullable: true);
        }
    }
}
