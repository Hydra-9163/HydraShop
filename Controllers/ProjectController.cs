using Microsoft.AspNetCore.Mvc;
using MachineShopManager.Data;
using MachineShopManager.Models;
using MachineShopManager.Enums;

namespace MachineShopManager.Controllers;

public class UpdateStatusRequest
{
    public int ProjectId { get; set; }

    public string Status { get; set; } = "";
}

public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var projects = _context.Projects
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return View(projects);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Project project)
    {
        if (!ModelState.IsValid)
            return View(project);

        project.Status = ProjectStatus.Recebido;
        project.CreatedAt = DateTime.UtcNow;

        project.DesiredDelivery = DateTime.SpecifyKind(
        project.DesiredDelivery,
        DateTimeKind.Utc);

        _context.Projects.Add(project);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    public IActionResult Edit(int id)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Project project)
    {
        if (id != project.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(project);

        var existingProject = _context.Projects.FirstOrDefault(p => p.Id == id);

        if (existingProject == null)
            return NotFound();

        existingProject.Code = project.Code;
        existingProject.ProjectName = project.ProjectName;
        existingProject.StudentName = project.StudentName;
        existingProject.StudentEmail = project.StudentEmail;
        existingProject.Phone = project.Phone;
        existingProject.ServiceType = project.ServiceType;
        existingProject.Status = project.Status;
        existingProject.DesiredDelivery = DateTime.SpecifyKind(
            project.DesiredDelivery,
            DateTimeKind.Utc
        );
        existingProject.Notes = project.Notes;

        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = project.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(int id, ProjectStatus status)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        project.Status = status;

        _context.SaveChanges();

        return RedirectToAction("Dashboard", "Operator");
    }

    [HttpPost]
    public IActionResult UpdateStatusAjax([FromBody] UpdateStatusRequest request)
    {
        var project = _context.Projects
            .FirstOrDefault(p => p.Id == request.ProjectId);

        if (project == null)
            return NotFound();

        if (!Enum.TryParse<ProjectStatus>(
                request.Status,
                out var newStatus))
        {
            return BadRequest();
        }

        project.Status = newStatus;

        _context.SaveChanges();

        return Ok();
    }
}