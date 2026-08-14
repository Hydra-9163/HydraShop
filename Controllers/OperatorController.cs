using Microsoft.AspNetCore.Mvc;
using MachineShopManager.Data;
using MachineShopManager.Enums;
using MachineShopManager.ViewModels;

namespace MachineShopManager.Controllers;

public class OperatorController : Controller
{
    private readonly ApplicationDbContext _context;

    public OperatorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        var vm = new OperatorDashboardViewModel();

        vm.Recebidos = _context.Projects
            .Where(p => p.Status == ProjectStatus.Recebido)
            .ToList();

        vm.EmAnalise = _context.Projects
            .Where(p => p.Status == ProjectStatus.EmAnalise)
            .ToList();

        vm.EmFila = _context.Projects
            .Where(p => p.Status == ProjectStatus.EmFila)
            .ToList();

        vm.AguardandoMaterial = _context.Projects
            .Where(p => p.Status == ProjectStatus.AguardandoMaterial)
            .ToList();

        vm.EmProducao = _context.Projects
            .Where(p => p.Status == ProjectStatus.EmProducao)
            .ToList();

        vm.Pausados = _context.Projects
            .Where(p => p.Status == ProjectStatus.Pausado)
            .ToList();

        vm.Prontos = _context.Projects
            .Where(p => p.Status == ProjectStatus.ProntoParaEntrega)
            .ToList();

        vm.Entregues = _context.Projects
            .Where(p => p.Status == ProjectStatus.Entregue)
            .ToList();

        vm.Cancelados = _context.Projects
            .Where(p => p.Status == ProjectStatus.Cancelado)
            .ToList();

        return View(vm);
    }
}