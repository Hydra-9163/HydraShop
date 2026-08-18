using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MachineShopManager.Migrations
{
    /// <inheritdoc />
    public partial class FixProjectHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldStatus",
                table: "ProjectHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectHistories",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousStatus",
                table: "ProjectHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousStatus",
                table: "ProjectHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProjectHistories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "OldStatus",
                table: "ProjectHistories",
                type: "integer",
                nullable: true);
        }
    }
}
