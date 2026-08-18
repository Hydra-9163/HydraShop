using System.ComponentModel.DataAnnotations;
using MachineShopManager.Enums;

namespace MachineShopManager.Models;

public class Project
{
    public int Id { get; set; }

    [Display(Name = "Código do Projeto")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Nome do Projeto")]
    public string ProjectName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Nome do Aluno")]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string StudentEmail { get; set; } = string.Empty;

    [Display(Name = "Telefone")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Tipo de Serviço")]
    public ServiceType ServiceType { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Recebido;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Entrega Desejada")]
    public DateTime DesiredDelivery { get; set; }

    [Display(Name = "Observações")]
    public string? Notes { get; set; }

    public bool Archived { get; set; } = false;

    public ICollection<ProjectPhoto> Photos { get; set; }
    = new List<ProjectPhoto>();

    public ICollection<ProjectHistory> History { get; set; }
    = new List<ProjectHistory>();

    public ProjectServiceRequirement? ServiceRequirement { get; set; }
}
