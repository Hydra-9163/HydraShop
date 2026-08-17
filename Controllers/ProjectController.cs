using Microsoft.AspNetCore.Mvc;
using MachineShopManager.Data;
using MachineShopManager.Models;
using MachineShopManager.Enums;
using Microsoft.EntityFrameworkCore;

namespace MachineShopManager.Controllers;

public class UpdateStatusRequest
{
    public int ProjectId { get; set; }

    public string Status { get; set; } = "";
}

public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProjectController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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
        var project = _context.Projects
            .Include(p => p.Photos)
            .FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    public IActionResult Edit(int id)
    {
        var project = _context.Projects
            .FirstOrDefault(p => p.Id == id);

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

        var existingProject = _context.Projects
            .FirstOrDefault(p => p.Id == id);

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

        return RedirectToAction(
            nameof(Details),
            new { id = project.Id }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(
        int id,
        ProjectStatus status)
    {
        var project = _context.Projects
            .FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        project.Status = status;

        _context.SaveChanges();

        return RedirectToAction(
            "Dashboard",
            "Operator"
        );
    }

    [HttpPost]
    public IActionResult UpdateStatusAjax(
        [FromBody] UpdateStatusRequest request)
    {
        if (request == null)
            return BadRequest(new
            {
                success = false,
                message = "Requisição inválida."
            });


        var project = _context.Projects
            .FirstOrDefault(p => p.Id == request.ProjectId);

        if (project == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Projeto não encontrado."
            });
        }


        if (!Enum.TryParse<ProjectStatus>(
                request.Status,
                out var newStatus))
        {
            return BadRequest(new
            {
                success = false,
                message = "Status inválido."
            });
        }


        // Não faz nada se o status já for o mesmo
        if (project.Status == newStatus)
        {
            return Ok(new
            {
                success = true,
                message = "O projeto já possui este status."
            });
        }


        // Atualiza o status
        project.Status = newStatus;

        _context.SaveChanges();


        return Ok(new
        {
            success = true,
            message = "Status atualizado com sucesso.",
            status = newStatus.ToString()
        });
    }

    // ==========================================
    // UPLOAD DE FOTO DO RELATÓRIO
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadPhoto(
        int projectId,
        IFormFile photo)
    {
        // Verifica se o projeto existe
        var project = _context.Projects
            .FirstOrDefault(p => p.Id == projectId);

        if (project == null)
            return NotFound();


        // Verifica se o arquivo foi enviado
        if (photo == null || photo.Length == 0)
        {
            TempData["Error"] = "Nenhuma foto foi selecionada.";

            return RedirectToAction(
                nameof(Details),
                new { id = projectId }
            );
        }


        // Extensões permitidas
        var allowedExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".png"
        };


        var extension = Path
            .GetExtension(photo.FileName)
            .ToLowerInvariant();


        if (!allowedExtensions.Contains(extension))
        {
            TempData["Error"] =
                "Formato inválido. Use JPG, JPEG ou PNG.";

            return RedirectToAction(
                nameof(Details),
                new { id = projectId }
            );
        }


        // Limite de tamanho: 10 MB
        const long maxFileSize = 10 * 1024 * 1024;

        if (photo.Length > maxFileSize)
        {
            TempData["Error"] =
                "A foto não pode ter mais de 10 MB.";

            return RedirectToAction(
                nameof(Details),
                new { id = projectId }
            );
        }


        // Cria uma pasta específica para o projeto
        var projectFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "projects",
            projectId.ToString()
        );


        if (!Directory.Exists(projectFolder))
        {
            Directory.CreateDirectory(projectFolder);
        }


        // Gera um nome único para evitar conflitos
        var uniqueFileName =
            $"{Guid.NewGuid()}{extension}";


        var filePath = Path.Combine(
            projectFolder,
            uniqueFileName
        );


        // Salva o arquivo
        using (var stream = new FileStream(
            filePath,
            FileMode.Create))
        {
            photo.CopyTo(stream);
        }


        // Caminho que será utilizado pelo navegador
        var relativePath =
            $"/uploads/projects/{projectId}/{uniqueFileName}";


        // Registra a foto no banco
        var projectPhoto = new ProjectPhoto
        {
            ProjectId = projectId,
            FileName = photo.FileName,
            FilePath = relativePath,
            UploadedAt = DateTime.UtcNow
        };


        _context.ProjectPhotos.Add(projectPhoto);

        _context.SaveChanges();


        TempData["Success"] =
            "Foto adicionada ao relatório com sucesso.";


        return RedirectToAction(
            nameof(Details),
            new { id = projectId }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePhoto(int id)
    {
        var photo = _context.ProjectPhotos
            .FirstOrDefault(p => p.Id == id);

        if (photo == null)
            return NotFound();

        var projectId = photo.ProjectId;

        // Remove o arquivo físico
        if (!string.IsNullOrEmpty(photo.FilePath))
        {
            var relativePath = photo.FilePath.TrimStart('/');

            var physicalPath = Path.Combine(
                _environment.WebRootPath,
                relativePath.Replace(
                    '/',
                    Path.DirectorySeparatorChar
                )
            );

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        // Remove o registro do banco
        _context.ProjectPhotos.Remove(photo);
        _context.SaveChanges();

        TempData["Success"] =
            "Foto removida do relatório.";

        return RedirectToAction(
            nameof(Details),
            new { id = projectId }
        );
    }
}