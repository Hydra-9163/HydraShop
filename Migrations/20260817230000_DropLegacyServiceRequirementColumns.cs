using Microsoft.EntityFrameworkCore.Migrations;

namespace MachineShopManager.Migrations;

/// <summary>
/// Remove as colunas antigas e genéricas criadas em AddProjectServiceRequirements
/// (RequiredFileName, RequiredFilePath, Value1, Value2, etc.). Elas ficaram como
/// NOT NULL sem valor padrão e não são mais preenchidas pelo modelo atual
/// (ProjectServiceRequirement usa campos semânticos como ThreeDFileName, PrintMaterial,
/// HoleDiameter etc.), o que fazia todo INSERT falhar por violação de NOT NULL
/// e impedia a criação de projetos.
/// </summary>
public partial class DropLegacyServiceRequirementColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "ProjectServiceRequirements"
            DROP COLUMN IF EXISTS "RequiredFileName",
            DROP COLUMN IF EXISTS "RequiredFilePath",
            DROP COLUMN IF EXISTS "OptionalFileName",
            DROP COLUMN IF EXISTS "OptionalFilePath",
            DROP COLUMN IF EXISTS "Value1",
            DROP COLUMN IF EXISTS "Value2",
            DROP COLUMN IF EXISTS "Value3",
            DROP COLUMN IF EXISTS "Value4",
            DROP COLUMN IF EXISTS "OptionalValue1",
            DROP COLUMN IF EXISTS "OptionalValue2",
            DROP COLUMN IF EXISTS "OptionalValue3";
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Colunas legadas descontinuadas: não há necessidade de recriá-las.
    }
}
