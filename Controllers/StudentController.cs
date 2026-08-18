using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MachineShopManager.Data;
using Microsoft.EntityFrameworkCore;

namespace MachineShopManager.Controllers;

public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }


    // ==========================================
    // Tela inicial do aluno
    // ==========================================

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
    }


    // ==========================================
    // Consulta de projeto pelo código
    // ==========================================

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Project(string code)
    {
        // Verifica se o código foi informado
        if (string.IsNullOrWhiteSpace(code))
        {
            TempData["Error"] = "Digite o código do projeto.";

            return RedirectToHomeWithStudentError("Digite o código do projeto.");
        }


        // Remove espaços antes/depois do código
        code = code.Trim().ToUpperInvariant();

        // Procura o projeto no banco
        var project = _context.Projects
            .Include(p => p.ServiceRequirement)
            .FirstOrDefault(p => p.Code == code);


        // Projeto não encontrado
        if (project == null)
        {
            return RedirectToHomeWithStudentError(
                "Código do projeto não encontrado. Verifique o código informado e tente novamente.");
        }


        // Projeto arquivado não deve aparecer para o aluno
        if (project.Archived)
        {
            return RedirectToHomeWithStudentError(
                "Este projeto não está mais disponível para consulta.");
        }


        // Projeto encontrado
        return View(project);
    }

    private IActionResult RedirectToHomeWithStudentError(string message)
    {
        TempData["StudentError"] = message;
        TempData["SelectedAccessMode"] = "student";
        return RedirectToAction("Index", "Home");
    }
}
