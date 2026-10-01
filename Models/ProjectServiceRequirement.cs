using MachineShopManager.Enums;

namespace MachineShopManager.Models;

/// <summary>Requisitos técnicos preenchidos para o serviço selecionado no projeto.</summary>
public class ProjectServiceRequirement
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public ServiceType ServiceType { get; set; }

    public string? ThreeDFileName { get; set; }
    public string? ThreeDFilePath { get; set; }
    public string? PrintMaterial { get; set; }
    public string? FilamentColor { get; set; }
    public string? Infill { get; set; }
    public string? PreferredOrientation { get; set; }
    public string? PostProcessing { get; set; }
    public string? CncModelFileName { get; set; }
    public string? CncModelFilePath { get; set; }
    public string? MaterialSpecification { get; set; }
    public int? Quantity { get; set; }
    public string? TechnicalDrawingFileName { get; set; }
    public string? TechnicalDrawingFilePath { get; set; }
    public string? SurfaceRoughness { get; set; }
    public string? PostTreatment { get; set; }
    public string? HolePositionFileName { get; set; }
    public string? HolePositionFilePath { get; set; }
    public string? HoleDiameter { get; set; }
    public string? HoleDepth { get; set; }
    public string? ThreadOrRecess { get; set; }
    public string? HoleTolerance { get; set; }
    public string? WeldingDrawingFileName { get; set; }
    public string? WeldingDrawingFilePath { get; set; }
    public string? BaseMaterials { get; set; }
    public string? WeldingProcess { get; set; }
    public string? WeldFinish { get; set; }
    public string? InspectionRequirement { get; set; }
    public string? BendingDrawingFileName { get; set; }
    public string? BendingDrawingFilePath { get; set; }
    public string? SheetMaterial { get; set; }
    public string? SheetThickness { get; set; }
    public string? BendAngles { get; set; }
    public string? InnerRadius { get; set; }
    public string? GrainDirection { get; set; }
    public string? VisualToleranceSide { get; set; }
    public string? CustomDescription { get; set; }
    public string? ReferenceFileName { get; set; }
    public string? ReferenceFilePath { get; set; }
    public string? MaximumDimensions { get; set; }
    public string? FinalApplication { get; set; }

    /// <summary>Arquivos enviados (vários por campo). Projetos antigos usam as colunas *FileName/*FilePath acima.</summary>
    public ICollection<ProjectRequirementFile> Files { get; set; } = new List<ProjectRequirementFile>();
}
