using System;
using MachineShopManager.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MachineShopManager.Migrations
{
    /// <summary>Cria a tabela que guarda vários arquivos por requisito de serviço.</summary>
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261001120000_AddProjectRequirementFiles")]
    public partial class AddProjectRequirementFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectRequirementFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectServiceRequirementId = table.Column<int>(type: "integer", nullable: false),
                    Field = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRequirementFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRequirementFiles_ProjectServiceRequirements_ProjectS~",
                        column: x => x.ProjectServiceRequirementId,
                        principalTable: "ProjectServiceRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRequirementFiles_ProjectServiceRequirementId",
                table: "ProjectRequirementFiles",
                column: "ProjectServiceRequirementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ProjectRequirementFiles");
        }
    }
}
