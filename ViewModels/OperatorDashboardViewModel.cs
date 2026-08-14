using MachineShopManager.Models;

namespace MachineShopManager.ViewModels;

public class OperatorDashboardViewModel
{
    public List<Project> Recebidos { get; set; } = new();

    public List<Project> EmAnalise { get; set; } = new();

    public List<Project> EmFila { get; set; } = new();

    public List<Project> AguardandoMaterial { get; set; } = new();

    public List<Project> EmProducao { get; set; } = new();

    public List<Project> Pausados { get; set; } = new();

    public List<Project> Prontos { get; set; } = new();

    public List<Project> Entregues { get; set; } = new();

    public List<Project> Cancelados { get; set; } = new();
}