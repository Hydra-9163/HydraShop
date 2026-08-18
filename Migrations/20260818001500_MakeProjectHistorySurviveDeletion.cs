using Microsoft.EntityFrameworkCore.Migrations;

namespace MachineShopManager.Migrations;

/// <summary>
/// Antes, excluir um projeto apagava (cascade) todo o seu ProjectHistory junto,
/// então não sobrava nenhum rastro de quem excluiu. Agora ProjectId vira opcional
/// e ganha ProjectCode (retrato do código no momento do evento), e o FK passa a
/// SET NULL em vez de CASCADE — assim o histórico geral continua mostrando o
/// registro de exclusão (com o operador e o código do projeto) mesmo depois que
/// o projeto some da tabela Projects.
/// </summary>
public partial class MakeProjectHistorySurviveDeletion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "ProjectHistories" DROP CONSTRAINT IF EXISTS "FK_ProjectHistories_Projects_ProjectId";

            ALTER TABLE "ProjectHistories" ALTER COLUMN "ProjectId" DROP NOT NULL;

            ALTER TABLE "ProjectHistories" ADD COLUMN IF NOT EXISTS "ProjectCode" text;

            UPDATE "ProjectHistories" h
            SET "ProjectCode" = p."Code"
            FROM "Projects" p
            WHERE h."ProjectId" = p."Id" AND h."ProjectCode" IS NULL;

            UPDATE "ProjectHistories" SET "ProjectCode" = '' WHERE "ProjectCode" IS NULL;

            ALTER TABLE "ProjectHistories" ALTER COLUMN "ProjectCode" SET NOT NULL;

            ALTER TABLE "ProjectHistories"
                ADD CONSTRAINT "FK_ProjectHistories_Projects_ProjectId"
                FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE SET NULL;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "ProjectHistories" DROP CONSTRAINT IF EXISTS "FK_ProjectHistories_Projects_ProjectId";

            DELETE FROM "ProjectHistories" WHERE "ProjectId" IS NULL;

            ALTER TABLE "ProjectHistories" ALTER COLUMN "ProjectId" SET NOT NULL;

            ALTER TABLE "ProjectHistories" DROP COLUMN IF EXISTS "ProjectCode";

            ALTER TABLE "ProjectHistories"
                ADD CONSTRAINT "FK_ProjectHistories_Projects_ProjectId"
                FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE;
            """);
    }
}
