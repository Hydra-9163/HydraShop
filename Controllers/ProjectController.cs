using Microsoft.AspNetCore.Mvc;
using MachineShopManager.Data;
using MachineShopManager.Models;
using MachineShopManager.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MachineShopManager.ViewModels;

namespace MachineShopManager.Controllers;

public class UpdateStatusRequest
{
    public int ProjectId { get; set; }

    public string Status { get; set; } = "";
}

[Authorize(Roles = "Operator")]
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
            .Where(p => !p.Archived)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return View(projects);
    }

    [HttpGet]
    public IActionResult Archived()
    {
        var projects = _context.Projects
            .Where(p => p.Archived)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return View(projects);
    }

    public IActionResult Create()
    {
        return View(new ProjectCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProjectCreateViewModel input)
    {
        var project = input.Project;
        if (!Enum.IsDefined(project.ServiceType))
            ModelState.AddModelError("Project.ServiceType", "Tipo de serviço inválido.");
        ValidateRequirements(project.ServiceType, input.Requirements);
        if (!ModelState.IsValid)
            return View(input);

        project.Status = ProjectStatus.Recebido;
        project.CreatedAt = DateTime.UtcNow;

        project.DesiredDelivery = DateTime.SpecifyKind(
            project.DesiredDelivery,
            DateTimeKind.Utc);

        using var transaction = _context.Database.BeginTransaction();

        _context.Projects.Add(project);

        _context.SaveChanges();

        _context.ProjectServiceRequirements.Add(CreateRequirement(project, input.Requirements));
        _context.SaveChanges();

        AddHistory(
            project,
            ProjectStatus.Recebido,
            ProjectStatus.Recebido,
            "Projeto criado.");

        _context.SaveChanges();

        // Gera 4 números aleatórios
        var random = Random.Shared.Next(0, 10000);

        // Monta o código usando o ID do projeto
        project.Code =
            $"HYDRA-{random:D4}-{project.Id:D3}";

        _context.SaveChanges();
        transaction.Commit();

        return RedirectToAction(
            nameof(Details),
            new { id = project.Id }
        );
    }

    public IActionResult Details(int id)
    {
        var project = _context.Projects
            .Include(p => p.Photos)
            .Include(p => p.History)
            .Include(p => p.ServiceRequirement)
            .FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        project.History = project.History
            .OrderByDescending(h => h.ChangedAt)
            .ToList();

        return View(project);
    }

    public IActionResult Edit(int id)
    {
        var project = _context.Projects
            .Include(p => p.ServiceRequirement)
            .FirstOrDefault(p => p.Id == id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(
        int id,
        [Bind("Id,ProjectName,StudentName,StudentEmail,Phone,ServiceType,Status,DesiredDelivery,Notes")]
        Project project)
    {
        if (id != project.Id)
            return NotFound();

        if (!Enum.IsDefined(project.Status))
        {
            ModelState.AddModelError(nameof(project.Status), "Status inv\u00e1lido.");
        }

        if (!ModelState.IsValid)
            return View(project);

        var existingProject = _context.Projects
            .FirstOrDefault(p => p.Id == id);

        if (existingProject == null)
            return NotFound();

        if (existingProject.Archived)
        {
            TempData["Error"] = "Desarquive o projeto antes de alter\u00e1-lo.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var desiredDelivery = DateTime.SpecifyKind(
            project.DesiredDelivery,
            DateTimeKind.Utc);

        var statusChanged = existingProject.Status != project.Status;
        var detailsChanged =
            existingProject.ProjectName != project.ProjectName ||
            existingProject.StudentName != project.StudentName ||
            existingProject.StudentEmail != project.StudentEmail ||
            existingProject.Phone != project.Phone ||
            existingProject.ServiceType != project.ServiceType ||
            existingProject.DesiredDelivery != desiredDelivery ||
            existingProject.Notes != project.Notes;

        var previousStatus = existingProject.Status;

        existingProject.ProjectName = project.ProjectName;
        existingProject.StudentName = project.StudentName;
        existingProject.StudentEmail = project.StudentEmail;
        existingProject.Phone = project.Phone;
        existingProject.ServiceType = project.ServiceType;
        existingProject.Status = project.Status;
        existingProject.DesiredDelivery = desiredDelivery;
        existingProject.Notes = project.Notes;

        if (statusChanged)
        {
            AddHistory(
                existingProject,
                previousStatus,
                project.Status,
                $"Status alterado de {GetStatusText(previousStatus)} para {GetStatusText(project.Status)}.");
        }

        if (detailsChanged)
        {
            AddHistory(
                existingProject,
                existingProject.Status,
                existingProject.Status,
                "Informa\u00e7\u00f5es do projeto atualizadas pelo operador.");
        }

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

        if (project.Archived)
        {
            TempData["Error"] = "Projetos arquivados n\u00e3o podem ter o status alterado.";
            return RedirectToAction(nameof(Details), new { id = project.Id });
        }

        if (!Enum.IsDefined(status))
        {
            TempData["Error"] = "Status inv\u00e1lido.";
            return RedirectToAction(nameof(Details), new { id = project.Id });
        }

        // Não registra nada se o status não mudou
        if (project.Status == status)
        {
            return RedirectToAction(
                "Details",
                new { id = project.Id }
            );
        }

        var previousStatus = project.Status;

        // Atualiza o status do projeto
        project.Status = status;

        // Registra a alteração no histórico
        AddHistory(project, previousStatus, status,
            $"Status alterado de {GetStatusText(previousStatus)} para {GetStatusText(status)}.");

        _context.SaveChanges();

        return RedirectToAction(
            "Details",
            new { id = project.Id }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatusAjax(
        [FromBody] UpdateStatusRequest request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Requisição inválida."
            });
        }

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

        if (project.Archived)
        {
            return BadRequest(new { success = false, message = "Projetos arquivados não podem ter o status alterado." });
        }

        if (!Enum.TryParse<ProjectStatus>(
                request.Status,
                ignoreCase: false,
                out var newStatus) || !Enum.IsDefined(newStatus))
        {
            return BadRequest(new
            {
                success = false,
                message = "Status inválido."
            });
        }

        // Não registra alteração se o status for igual
        if (project.Status == newStatus)
        {
            return Ok(new
            {
                success = true,
                message = "O projeto já possui este status."
            });
        }

        var previousStatus = project.Status;

        // Atualiza o projeto
        project.Status = newStatus;

        // Cria registro no histórico
        AddHistory(project, previousStatus, newStatus,
            $"Status alterado de {GetStatusText(previousStatus)} para {GetStatusText(newStatus)}.");

        _context.SaveChanges();

        return Ok(new
        {
            success = true,
            message = "Status atualizado com sucesso.",
            status = newStatus.ToString()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Archive(int id)
    {
        var project = _context.Projects.FirstOrDefault(project => project.Id == id);
        if (project == null) return NotFound();

        if (!project.Archived)
        {
            project.Archived = true;
            AddHistory(project, project.Status, project.Status, "Projeto arquivado pelo operador.");
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Archived));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Restore(int id)
    {
        var project = _context.Projects.FirstOrDefault(project => project.Id == id);
        if (project == null) return NotFound();

        if (project.Archived)
        {
            project.Archived = false;
            AddHistory(project, project.Status, project.Status, "Projeto desarquivado pelo operador.");
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var project = _context.Projects
            .Include(p => p.Photos)
            .FirstOrDefault(p => p.Id == id);
        if (project == null) return NotFound();

        var projectCode = project.Code;

        AddHistory(project, project.Status, project.Status, $"Projeto {projectCode} excluído permanentemente pelo operador.");
        _context.SaveChanges();

        var projectFolder = Path.Combine(_environment.WebRootPath, "uploads", "projects", project.Id.ToString());
        if (Directory.Exists(projectFolder))
        {
            Directory.Delete(projectFolder, recursive: true);
        }

        _context.Projects.Remove(project);
        _context.SaveChanges();

        TempData["Success"] = $"Projeto {projectCode} excluído permanentemente.";
        return RedirectToAction(nameof(Index));
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

    private void ValidateRequirements(ServiceType serviceType, ServiceRequirementInputModel requirement)
    {
        void Required(string? value, string key, string label)
        {
            if (string.IsNullOrWhiteSpace(value)) ModelState.AddModelError(key, $"{label} é obrigatório.");
        }
        void RequiredFile(IFormFile? file, string key, string label, string[] extensions)
        {
            if (file == null || file.Length == 0) ModelState.AddModelError(key, $"{label} é obrigatório.");
            ValidateFile(file, extensions, key);
        }

        switch (serviceType)
        {
            case ServiceType.Impressao3D:
                RequiredFile(requirement.ThreeDFile, "Requirements.ThreeDFile", "Arquivo 3D", new[] { ".stl", ".step", ".obj" });
                Required(requirement.PrintMaterial, "Requirements.PrintMaterial", "Material"); Required(requirement.FilamentColor, "Requirements.FilamentColor", "Cor do filamento/resina"); break;
            case ServiceType.UsinagemCNC:
                RequiredFile(requirement.CncModelFile, "Requirements.CncModelFile", "Modelo 3D", new[] { ".step", ".iges", ".igs" });
                Required(requirement.MaterialSpecification, "Requirements.MaterialSpecification", "Material");
                if (requirement.Quantity is null or < 1) ModelState.AddModelError("Requirements.Quantity", "Informe uma quantidade inteira maior que zero.");
                ValidateFile(requirement.TechnicalDrawing, new[] { ".pdf" }, "Requirements.TechnicalDrawing"); break;
            case ServiceType.Furacao:
                RequiredFile(requirement.HolePositionFile, "Requirements.HolePositionFile", "Posição das furações", new[] { ".pdf", ".stl", ".step", ".obj" });
                Required(requirement.HoleDiameter, "Requirements.HoleDiameter", "Diâmetro do furo"); Required(requirement.HoleDepth, "Requirements.HoleDepth", "Profundidade"); break;
            case ServiceType.Solda:
                RequiredFile(requirement.WeldingDrawing, "Requirements.WeldingDrawing", "Desenho/croqui da montagem", new[] { ".pdf", ".jpg", ".jpeg", ".png" });
                Required(requirement.BaseMaterials, "Requirements.BaseMaterials", "Materiais de base"); Required(requirement.WeldingProcess, "Requirements.WeldingProcess", "Processo de soldagem"); break;
            case ServiceType.Dobra:
                RequiredFile(requirement.BendingDrawing, "Requirements.BendingDrawing", "Desenho 2D/planificação", new[] { ".pdf", ".step" });
                Required(requirement.SheetMaterial, "Requirements.SheetMaterial", "Material"); Required(requirement.SheetThickness, "Requirements.SheetThickness", "Espessura"); Required(requirement.BendAngles, "Requirements.BendAngles", "Ângulo(s) de dobra"); Required(requirement.InnerRadius, "Requirements.InnerRadius", "Raio interno"); break;
            case ServiceType.Outro:
                RequiredFile(requirement.ReferenceFile, "Requirements.ReferenceFile", "Croqui, foto ou arquivo base", new[] { ".pdf", ".jpg", ".jpeg", ".png", ".stl", ".step", ".obj" });
                Required(requirement.CustomDescription, "Requirements.CustomDescription", "Descrição detalhada"); Required(requirement.MaximumDimensions, "Requirements.MaximumDimensions", "Dimensões brutas máximas"); break;
        }
    }

    private void ValidateFile(IFormFile? file, string[] extensions, string key)
    {
        if (file == null || file.Length == 0) return;
        if (file.Length > 10 * 1024 * 1024)
            ModelState.AddModelError(key, "O arquivo não pode exceder 10 MB.");
        else if (!extensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            ModelState.AddModelError(key, $"Formato inválido. Permitidos: {string.Join(", ", extensions)}.");
    }

    private ProjectServiceRequirement CreateRequirement(Project project, ServiceRequirementInputModel input)
    {
        var requirement = new ProjectServiceRequirement { ProjectId = project.Id, ServiceType = project.ServiceType };
        void File(IFormFile? file, Action<string, string> set) { if (file is { Length: > 0 }) { var saved = SaveRequirementFile(project.Id, file); set(saved.name, saved.path); } }
        string? Text(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        switch (project.ServiceType)
        {
            case ServiceType.Impressao3D: File(input.ThreeDFile, (n,p) => { requirement.ThreeDFileName=n; requirement.ThreeDFilePath=p; }); requirement.PrintMaterial=Text(input.PrintMaterial); requirement.FilamentColor=Text(input.FilamentColor); requirement.Infill=Text(input.Infill); requirement.PreferredOrientation=Text(input.PreferredOrientation); requirement.PostProcessing=Text(input.PostProcessing); break;
            case ServiceType.UsinagemCNC: File(input.CncModelFile, (n,p) => { requirement.CncModelFileName=n; requirement.CncModelFilePath=p; }); File(input.TechnicalDrawing, (n,p) => { requirement.TechnicalDrawingFileName=n; requirement.TechnicalDrawingFilePath=p; }); requirement.MaterialSpecification=Text(input.MaterialSpecification); requirement.Quantity=input.Quantity; requirement.SurfaceRoughness=Text(input.SurfaceRoughness); requirement.PostTreatment=Text(input.PostTreatment); break;
            case ServiceType.Furacao: File(input.HolePositionFile, (n,p) => { requirement.HolePositionFileName=n; requirement.HolePositionFilePath=p; }); requirement.HoleDiameter=Text(input.HoleDiameter); requirement.HoleDepth=Text(input.HoleDepth); requirement.ThreadOrRecess=Text(input.ThreadOrRecess); requirement.HoleTolerance=Text(input.HoleTolerance); break;
            case ServiceType.Solda: File(input.WeldingDrawing, (n,p) => { requirement.WeldingDrawingFileName=n; requirement.WeldingDrawingFilePath=p; }); requirement.BaseMaterials=Text(input.BaseMaterials); requirement.WeldingProcess=Text(input.WeldingProcess); requirement.WeldFinish=Text(input.WeldFinish); requirement.InspectionRequirement=Text(input.InspectionRequirement); break;
            case ServiceType.Dobra: File(input.BendingDrawing, (n,p) => { requirement.BendingDrawingFileName=n; requirement.BendingDrawingFilePath=p; }); requirement.SheetMaterial=Text(input.SheetMaterial); requirement.SheetThickness=Text(input.SheetThickness); requirement.BendAngles=Text(input.BendAngles); requirement.InnerRadius=Text(input.InnerRadius); requirement.GrainDirection=Text(input.GrainDirection); requirement.VisualToleranceSide=Text(input.VisualToleranceSide); break;
            case ServiceType.Outro: File(input.ReferenceFile, (n,p) => { requirement.ReferenceFileName=n; requirement.ReferenceFilePath=p; }); requirement.CustomDescription=Text(input.CustomDescription); requirement.MaximumDimensions=Text(input.MaximumDimensions); requirement.FinalApplication=Text(input.FinalApplication); break;
        }
        return requirement;
    }

    private (string name, string path) SaveRequirementFile(int projectId, IFormFile file)
    {
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "projects", projectId.ToString(), "requirements");
        Directory.CreateDirectory(folder);
        var storedName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
        using var stream = new FileStream(Path.Combine(folder, storedName), FileMode.Create);
        file.CopyTo(stream);
        return (file.FileName, $"/uploads/projects/{projectId}/requirements/{storedName}");
    }

    private void AddHistory(Project project, ProjectStatus previousStatus, ProjectStatus newStatus, string description)
    {
        _context.ProjectHistories.Add(new ProjectHistory
        {
            ProjectId = project.Id,
            ProjectCode = project.Code,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            Description = description,
            OperatorEmail = User.Identity?.Name,
            ChangedAt = DateTime.UtcNow
        });
    }

    private static string GetStatusText(ProjectStatus status)
    {
        return status switch
        {
            ProjectStatus.Recebido => "Recebido",
            ProjectStatus.EmAnalise => "Em análise",
            ProjectStatus.EmFila => "Em fila",
            ProjectStatus.AguardandoMaterial => "Aguardando material",
            ProjectStatus.EmProducao => "Em produção",
            ProjectStatus.Pausado => "Pausado",
            ProjectStatus.ProntoParaEntrega => "Pronto para entrega",
            ProjectStatus.Entregue => "Entregue",
            ProjectStatus.Cancelado => "Cancelado",
            _ => "Desconhecido"
        };
    }
}