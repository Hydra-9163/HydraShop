using Microsoft.AspNetCore.Mvc;
using MachineShopManager.Data;
using MachineShopManager.Enums;
using MachineShopManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MachineShopManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace MachineShopManager.Controllers;

[Authorize(Roles = "Operator")]
public class OperatorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public OperatorController(
        ApplicationDbContext context,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // =========================
    // LOGIN
    // =========================

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Operator"))
            return RedirectToAction(nameof(Dashboard));

        return RedirectToAction("Index", "Home", new { mode = "operator", returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email,
        string password,
        string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return RedirectToHomeWithOperatorError("E-mail e senha são obrigatórios.", returnUrl);
        }

        var user = await _userManager.FindByEmailAsync(email.Trim());

        // Somente identidades atribuídas explicitamente ao papel de operador
        // podem receber um cookie de autenticação administrativa.
        if (user == null || !await _userManager.IsInRoleAsync(user, "Operator"))
        {
            return RedirectToHomeWithOperatorError("E-mail ou senha incorretos. Verifique suas credenciais e tente novamente.", returnUrl);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Dashboard");
        }

        return RedirectToHomeWithOperatorError("E-mail ou senha incorretos. Verifique suas credenciais e tente novamente.", returnUrl);
    }

    // =========================
    // LOGOUT
    // =========================

    [Authorize(Roles = "Operator")]
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

        vm.EmFila = _context.Projects
            .Where(p => !p.Archived && p.Status == ProjectStatus.EmFila)
            .OrderBy(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .ToList();

        vm.EmProducao = _context.Projects
            .Where(p => !p.Archived && p.Status == ProjectStatus.EmProducao)
            .ToList();

        vm.Pausados = _context.Projects
            .Where(p => !p.Archived && p.Status == ProjectStatus.Pausado)
            .ToList();

        vm.Prontos = _context.Projects
            .Where(p => !p.Archived && p.Status == ProjectStatus.ProntoParaEntrega)
            .ToList();

        vm.Entregues = _context.Projects
            .Where(p => !p.Archived && p.Status == ProjectStatus.Entregue)
            .ToList();

        vm.Cancelados = _context.Projects
            .Where(p => !p.Archived && p.Status == ProjectStatus.Cancelado)
            .ToList();

        return View(vm);
    }

    // =========================
    // ESTATÍSTICAS
    // =========================

    [HttpGet]
    public async Task<IActionResult> Statistics()
    {
        var totalProjects = await _context.Projects
            .AsNoTracking()
            .CountAsync();

        var archivedProjects = await _context.Projects
            .AsNoTracking()
            .CountAsync(project => project.Archived);

        var statusCounts = await _context.Projects
            .AsNoTracking()
            .Where(project => !project.Archived)
            .GroupBy(project => project.Status)
            .ToDictionaryAsync(group => group.Key, group => group.Count());

        var statusStatistics = Enum
            .GetValues<ProjectStatus>()
            .Select(status => new ProjectStatusStatisticViewModel
            {
                Status = status,
                Label = GetStatusLabel(status),
                Count = statusCounts.GetValueOrDefault(status)
            })
            .ToList();

        var history = await _context.ProjectHistories
            .AsNoTracking()
            .OrderByDescending(item => item.ChangedAt)
            .Take(100)
            .Select(item => new ProjectHistoryListItemViewModel
            {
                ProjectCode = item.ProjectCode,
                Description = item.Description,
                OperatorEmail = item.OperatorEmail,
                ChangedAt = item.ChangedAt
            })
            .ToListAsync();

        var viewModel = new OperatorStatisticsViewModel
        {
            TotalProjects = totalProjects,
            ArchivedProjects = archivedProjects,
            ActiveProjects = totalProjects - archivedProjects,
            Statuses = statusStatistics,
            History = history
        };

        return View(viewModel);
    }

    // =========================
    // CONFIGURAÇÕES DA CONTA
    // =========================

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        return View(await BuildSettingsViewModelAsync(user));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(
        [Bind(Prefix = "Profile")] OperatorProfileInputModel profile)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (!ModelState.IsValid)
            return await SettingsViewAsync(profile: profile);

        var email = profile.Email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null && existingUser.Id != user.Id)
        {
            ModelState.AddModelError(
                "Profile.Email",
                "Este e-mail já está em uso por outra conta.");

            return await SettingsViewAsync(profile: profile);
        }

        // O login atual usa o nome de usuário; por isso ele é mantido
        // sincronizado com o e-mail informado pelo operador.
        user.Email = email;
        user.UserName = email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            AddIdentityErrors(result, "Profile.Email");
            return await SettingsViewAsync(profile: profile);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "E-mail da conta atualizado com sucesso.";
        return RedirectToAction(nameof(Settings));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        [Bind(Prefix = "Password")] ChangeOperatorPasswordInputModel password)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (!ModelState.IsValid)
            return await SettingsViewAsync(password: password);

        var result = await _userManager.ChangePasswordAsync(
            user,
            password.CurrentPassword,
            password.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                var key = error.Code == "PasswordMismatch"
                    ? "Password.CurrentPassword"
                    : "Password.NewPassword";

                ModelState.AddModelError(key, error.Description);
            }

            return await SettingsViewAsync(password: password);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "Senha alterada com sucesso.";
        return RedirectToAction(nameof(Settings));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOperator(
        [Bind(Prefix = "NewOperator")] CreateOperatorInputModel newOperator)
    {
        if (!ModelState.IsValid)
            return await SettingsViewAsync(newOperator: newOperator);

        var email = newOperator.Email.Trim();
        if (await _userManager.FindByEmailAsync(email) != null)
        {
            ModelState.AddModelError("NewOperator.Email", "Este e-mail já está em uso por outra conta.");
            return await SettingsViewAsync(newOperator: newOperator);
        }

        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, newOperator.Password);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result, "NewOperator.Password");
            return await SettingsViewAsync(newOperator: newOperator);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Operator");
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            AddIdentityErrors(roleResult, "NewOperator.Email");
            return await SettingsViewAsync(newOperator: newOperator);
        }

        TempData["Success"] = "Operador criado com sucesso.";
        return RedirectToAction(nameof(Settings));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeOtherOperatorPassword(
        [Bind(Prefix = "OtherOperatorPassword")] ChangeOtherOperatorPasswordInputModel password)
    {
        if (!ModelState.IsValid)
            return await SettingsViewAsync(otherOperatorPassword: password);

        var operatorToChange = await _userManager.FindByIdAsync(password.OperatorId);
        if (operatorToChange == null || !await _userManager.IsInRoleAsync(operatorToChange, "Operator"))
        {
            ModelState.AddModelError("OtherOperatorPassword.OperatorId", "Operador não encontrado.");
            return await SettingsViewAsync(otherOperatorPassword: password);
        }

        if (!await _userManager.CheckPasswordAsync(operatorToChange, password.CurrentPassword))
        {
            ModelState.AddModelError("OtherOperatorPassword.CurrentPassword", "Senha incorreta. Não foi possível autorizar a alteração.");
            return await SettingsViewAsync(otherOperatorPassword: password);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(operatorToChange);
        var result = await _userManager.ResetPasswordAsync(operatorToChange, token, password.NewPassword);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result, "OtherOperatorPassword.NewPassword");
            return await SettingsViewAsync(otherOperatorPassword: password);
        }

        TempData["Success"] = "Senha do operador atualizada com sucesso.";
        return RedirectToAction(nameof(Settings));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOperator(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        if (string.IsNullOrWhiteSpace(id) || id == currentUser.Id)
        {
            TempData["Error"] = "Você não pode excluir a conta do operador atualmente conectado.";
            return RedirectToAction(nameof(Settings));
        }

        var userToDelete = await _userManager.FindByIdAsync(id);
        if (userToDelete == null || !await _userManager.IsInRoleAsync(userToDelete, "Operator"))
        {
            TempData["Error"] = "Operador não encontrado.";
            return RedirectToAction(nameof(Settings));
        }

        var result = await _userManager.DeleteAsync(userToDelete);
        if (!result.Succeeded)
        {
            TempData["Error"] = "Não foi possível remover o operador.";
            return RedirectToAction(nameof(Settings));
        }

        TempData["Success"] = "Operador removido com sucesso.";
        return RedirectToAction(nameof(Settings));
    }

    private async Task<IActionResult> SettingsViewAsync(
        OperatorProfileInputModel? profile = null,
        ChangeOperatorPasswordInputModel? password = null,
        CreateOperatorInputModel? newOperator = null,
        ChangeOtherOperatorPasswordInputModel? otherOperatorPassword = null)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();
        return View("Settings", await BuildSettingsViewModelAsync(currentUser, profile, password, newOperator, otherOperatorPassword));
    }

    private async Task<OperatorSettingsViewModel> BuildSettingsViewModelAsync(
        ApplicationUser currentUser,
        OperatorProfileInputModel? profile = null,
        ChangeOperatorPasswordInputModel? password = null,
        CreateOperatorInputModel? newOperator = null,
        ChangeOtherOperatorPasswordInputModel? otherOperatorPassword = null)
    {
        var operators = await _userManager.GetUsersInRoleAsync("Operator");
        return new OperatorSettingsViewModel
        {
            Profile = profile ?? new OperatorProfileInputModel { Email = currentUser.Email ?? string.Empty },
            Password = password ?? new ChangeOperatorPasswordInputModel(),
            NewOperator = newOperator ?? new CreateOperatorInputModel(),
            OtherOperatorPassword = otherOperatorPassword ?? new ChangeOtherOperatorPasswordInputModel(),
            OtherOperators = operators
                .Where(user => user.Id != currentUser.Id)
                .OrderBy(user => user.Email)
                .Select(user => new OperatorListItemViewModel { Id = user.Id, Email = user.Email ?? user.UserName ?? "Sem e-mail" })
                .ToList()
        };
    }

    private IActionResult RedirectToHomeWithOperatorError(string message, string? returnUrl)
    {
        TempData["OperatorError"] = message;
        TempData["SelectedAccessMode"] = "operator";
        return RedirectToAction("Index", "Home", new { mode = "operator", returnUrl });
    }

    private void AddIdentityErrors(IdentityResult result, string modelKey)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(modelKey, error.Description);
        }
    }

    private static string GetStatusLabel(ProjectStatus status)
    {
        return status switch
        {
            ProjectStatus.EmFila => "Em fila",
            ProjectStatus.EmProducao => "Em produção",
            ProjectStatus.Pausado => "Pausado",
            ProjectStatus.ProntoParaEntrega => "Pronto para entrega",
            ProjectStatus.Entregue => "Entregue",
            ProjectStatus.Cancelado => "Cancelado",
            _ => status.ToString()
        };
    }
}
