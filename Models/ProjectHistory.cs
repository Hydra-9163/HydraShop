using MachineShopManager.Enums;
using System.ComponentModel.DataAnnotations;

namespace MachineShopManager.Models;

public class ProjectHistory
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public Project? Project { get; set; }

    // Guarda o código do projeto no momento do registro, para que o
    // histórico geral continue legível mesmo depois que o projeto for excluído.
    public string ProjectCode { get; set; } = string.Empty;

    public ProjectStatus PreviousStatus { get; set; }

    public ProjectStatus NewStatus { get; set; }

    public string Description { get; set; } = string.Empty;

    // Keep the audit entry independent from an Identity user record so that
    // historic data remains available even if the account is later changed.
    public string? OperatorEmail { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}