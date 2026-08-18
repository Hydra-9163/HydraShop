using MachineShopManager.Enums;

namespace MachineShopManager.ViewModels;

/// <summary>
/// Dados agregados do banco para a visão administrativa de estatísticas.
/// Os totais por status consideram somente projetos ativos; os arquivados
/// permanecem disponíveis no total geral e em seu indicador próprio.
/// </summary>
public sealed class OperatorStatisticsViewModel
{
    public int TotalProjects { get; init; }

    public int ActiveProjects { get; init; }

    public int ArchivedProjects { get; init; }

    public IReadOnlyList<ProjectStatusStatisticViewModel> Statuses { get; init; }
        = Array.Empty<ProjectStatusStatisticViewModel>();

    public IReadOnlyList<ProjectHistoryListItemViewModel> History { get; init; }
        = Array.Empty<ProjectHistoryListItemViewModel>();
}

public sealed class ProjectHistoryListItemViewModel
{
    public string ProjectCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? OperatorEmail { get; init; }
    public DateTime ChangedAt { get; init; }
}

public sealed class ProjectStatusStatisticViewModel
{
    public ProjectStatus Status { get; init; }

    public string Label { get; init; } = string.Empty;

    public int Count { get; init; }
}
