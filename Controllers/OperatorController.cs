using Microsoft.AspNetCore.Mvc;
using MachineShopManager.Data;
using MachineShopManager.Enums;
using MachineShopManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MachineShopManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace MachineShopManager.Controllers;

public class OperatorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public OperatorController(
        ApplicationDbContext context,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }

    // =========================
    // LOGIN
    // =========================

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Erro = "Email e senha são obrigatórios.";
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToAction("Dashboard");
        }

        ViewBag.Erro = "E-mail ou senha incorretos.";
        return View();
    }

    // =========================
    // LOGOUT
    // =========================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    // =========================
    // DASHBOARD
    // =========================

    [Authorize(Roles = "Operator")]
    [HttpGet]
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

    // =========================
    // DETALHES DO PROJETO
    // =========================

    [Authorize(Roles = "Operator")]
    public IActionResult Details(int id)
    {
        var project = _context.Projects
            .Include(p => p.Photos)
            .FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }
}