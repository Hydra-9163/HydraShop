using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MachineShopManager.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectHistoryOperatorEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperatorEmail",
                table: "ProjectHistories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperatorEmail",
                table: "ProjectHistories");
        }
    }
}
