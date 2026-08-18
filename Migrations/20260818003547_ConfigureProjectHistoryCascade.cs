using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MachineShopManager.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureProjectHistoryCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHistories_Projects_ProjectId",
                table: "ProjectHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHistories_Projects_ProjectId",
                table: "ProjectHistories",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHistories_Projects_ProjectId",
                table: "ProjectHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHistories_Projects_ProjectId",
                table: "ProjectHistories",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
