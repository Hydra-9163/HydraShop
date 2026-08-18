using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MachineShopManager.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectServiceRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectServiceRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ServiceType = table.Column<int>(type: "integer", nullable: false),
                    RequiredFileName = table.Column<string>(type: "text", nullable: false),
                    RequiredFilePath = table.Column<string>(type: "text", nullable: false),
                    OptionalFileName = table.Column<string>(type: "text", nullable: true),
                    OptionalFilePath = table.Column<string>(type: "text", nullable: true),
                    Value1 = table.Column<string>(type: "text", nullable: false),
                    Value2 = table.Column<string>(type: "text", nullable: false),
                    Value3 = table.Column<string>(type: "text", nullable: true),
                    Value4 = table.Column<string>(type: "text", nullable: true),
                    OptionalValue1 = table.Column<string>(type: "text", nullable: true),
                    OptionalValue2 = table.Column<string>(type: "text", nullable: true),
                    OptionalValue3 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectServiceRequirements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceRequirements_ProjectId",
                table: "ProjectServiceRequirements",
                column: "ProjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectServiceRequirements");
        }
    }
}
