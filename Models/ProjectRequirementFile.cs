namespace MachineShopManager.Models;

/// <summary>Arquivo anexado a um requisito de serviço. Um requisito pode ter vários arquivos.</summary>
public class ProjectRequirementFile
{
    public int Id { get; set; }

    public int ProjectServiceRequirementId { get; set; }

    public ProjectServiceRequirement ServiceRequirement { get; set; } = null!;

    /// <summary>Qual campo do formulário recebeu o arquivo (ex.: ThreeDFile, CncModelFile, TechnicalDrawing).</summary>
    public string Field { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
