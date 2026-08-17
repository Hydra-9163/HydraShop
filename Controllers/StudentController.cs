using Microsoft.AspNetCore.Mvc;
using MachineShopManager.Data;

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

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }


    // ==========================================
    // Consulta de projeto pelo código
    // ==========================================

    [HttpGet]
    public IActionResult Project(string code)
    {
        // Verifica se o código foi informado
        if (string.IsNullOrWhiteSpace(code))
        {
            TempData["Error"] = "Digite o código do projeto.";

            return RedirectToAction(nameof(Index));
        }


        // Remove espaços antes/depois do código
        code = code.Trim();


        // Procura o projeto no banco
        var project = _context.Projects
            .FirstOrDefault(p => p.Code == code);


        // Projeto não encontrado
        if (project == null)
        {
            TempData["Error"] =
                "Projeto não encontrado. Verifique o código informado.";

            return RedirectToAction(nameof(Index));
        }


        // Projeto arquivado não deve aparecer para o aluno
        if (project.Archived)
        {
            TempData["Error"] =
                "Este projeto não está mais disponível para consulta.";

            return RedirectToAction(nameof(Index));
        }


        // Projeto encontrado
        return View(project);
    }
}