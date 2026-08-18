using Microsoft.EntityFrameworkCore.Migrations;

namespace MachineShopManager.Migrations;

public partial class AddSemanticServiceRequirementFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "ProjectServiceRequirements"
            ADD COLUMN IF NOT EXISTS "ThreeDFileName" text,
            ADD COLUMN IF NOT EXISTS "ThreeDFilePath" text,
            ADD COLUMN IF NOT EXISTS "PrintMaterial" text,
            ADD COLUMN IF NOT EXISTS "FilamentColor" text,
            ADD COLUMN IF NOT EXISTS "Infill" text,
            ADD COLUMN IF NOT EXISTS "PreferredOrientation" text,
            ADD COLUMN IF NOT EXISTS "PostProcessing" text,
            ADD COLUMN IF NOT EXISTS "CncModelFileName" text,
            ADD COLUMN IF NOT EXISTS "CncModelFilePath" text,
            ADD COLUMN IF NOT EXISTS "MaterialSpecification" text,
            ADD COLUMN IF NOT EXISTS "Quantity" integer,
            ADD COLUMN IF NOT EXISTS "TechnicalDrawingFileName" text,
            ADD COLUMN IF NOT EXISTS "TechnicalDrawingFilePath" text,
            ADD COLUMN IF NOT EXISTS "SurfaceRoughness" text,
            ADD COLUMN IF NOT EXISTS "PostTreatment" text,
            ADD COLUMN IF NOT EXISTS "HolePositionFileName" text,
            ADD COLUMN IF NOT EXISTS "HolePositionFilePath" text,
            ADD COLUMN IF NOT EXISTS "HoleDiameter" text,
            ADD COLUMN IF NOT EXISTS "HoleDepth" text,
            ADD COLUMN IF NOT EXISTS "ThreadOrRecess" text,
            ADD COLUMN IF NOT EXISTS "HoleTolerance" text,
            ADD COLUMN IF NOT EXISTS "WeldingDrawingFileName" text,
            ADD COLUMN IF NOT EXISTS "WeldingDrawingFilePath" text,
            ADD COLUMN IF NOT EXISTS "BaseMaterials" text,
            ADD COLUMN IF NOT EXISTS "WeldingProcess" text,
            ADD COLUMN IF NOT EXISTS "WeldFinish" text,
            ADD COLUMN IF NOT EXISTS "InspectionRequirement" text,
            ADD COLUMN IF NOT EXISTS "BendingDrawingFileName" text,
            ADD COLUMN IF NOT EXISTS "BendingDrawingFilePath" text,
            ADD COLUMN IF NOT EXISTS "SheetMaterial" text,
            ADD COLUMN IF NOT EXISTS "SheetThickness" text,
            ADD COLUMN IF NOT EXISTS "BendAngles" text,
            ADD COLUMN IF NOT EXISTS "InnerRadius" text,
            ADD COLUMN IF NOT EXISTS "GrainDirection" text,
            ADD COLUMN IF NOT EXISTS "VisualToleranceSide" text,
            ADD COLUMN IF NOT EXISTS "CustomDescription" text,
            ADD COLUMN IF NOT EXISTS "ReferenceFileName" text,
            ADD COLUMN IF NOT EXISTS "ReferenceFilePath" text,
            ADD COLUMN IF NOT EXISTS "MaximumDimensions" text,
            ADD COLUMN IF NOT EXISTS "FinalApplication" text;
            """);
    }
    protected override void Down(MigrationBuilder migrationBuilder) { }
}
