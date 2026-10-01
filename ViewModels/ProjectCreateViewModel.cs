using MachineShopManager.Models;

namespace MachineShopManager.ViewModels;

public sealed class ProjectCreateViewModel
{
    public Project Project { get; set; } = new();
    public ServiceRequirementInputModel Requirements { get; set; } = new();
}

public sealed class ServiceRequirementInputModel
{
    public List<IFormFile>? ThreeDFile { get; set; }
    public string? PrintMaterial { get; set; }
    public string? FilamentColor { get; set; }
    public string? Infill { get; set; }
    public string? PreferredOrientation { get; set; }
    public string? PostProcessing { get; set; }
    public List<IFormFile>? CncModelFile { get; set; }
    public string? MaterialSpecification { get; set; }
    public int? Quantity { get; set; }
    public List<IFormFile>? TechnicalDrawing { get; set; }
    public string? SurfaceRoughness { get; set; }
    public string? PostTreatment { get; set; }
    public List<IFormFile>? HolePositionFile { get; set; }
    public string? HoleDiameter { get; set; }
    public string? HoleDepth { get; set; }
    public string? ThreadOrRecess { get; set; }
    public string? HoleTolerance { get; set; }
    public List<IFormFile>? WeldingDrawing { get; set; }
    public string? BaseMaterials { get; set; }
    public string? WeldingProcess { get; set; }
    public string? WeldFinish { get; set; }
    public string? InspectionRequirement { get; set; }
    public List<IFormFile>? BendingDrawing { get; set; }
    public string? SheetMaterial { get; set; }
    public string? SheetThickness { get; set; }
    public string? BendAngles { get; set; }
    public string? InnerRadius { get; set; }
    public string? GrainDirection { get; set; }
    public string? VisualToleranceSide { get; set; }
    public string? CustomDescription { get; set; }
    public List<IFormFile>? ReferenceFile { get; set; }
    public string? MaximumDimensions { get; set; }
    public string? FinalApplication { get; set; }
}
