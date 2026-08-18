using System.ComponentModel.DataAnnotations;

namespace MachineShopManager.ViewModels;

public sealed class OperatorSettingsViewModel
{
    public OperatorProfileInputModel Profile { get; init; } = new();

    public ChangeOperatorPasswordInputModel Password { get; init; } = new();

    public CreateOperatorInputModel NewOperator { get; init; } = new();

    public ChangeOtherOperatorPasswordInputModel OtherOperatorPassword { get; init; } = new();

    public IReadOnlyList<OperatorListItemViewModel> OtherOperators { get; init; }
        = Array.Empty<OperatorListItemViewModel>();
}

public sealed class OperatorProfileInputModel
{
    [Required(ErrorMessage = "Informe o e-mail do operador.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;
}

public sealed class ChangeOperatorPasswordInputModel
{
    [Required(ErrorMessage = "Informe a senha atual.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha atual")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "A nova senha deve ter pelo menos {2} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a nova senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "A confirmação não corresponde à nova senha.")]
    [Display(Name = "Confirmar nova senha")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public sealed class CreateOperatorInputModel
{
    [Required(ErrorMessage = "Informe o e-mail do operador.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe uma senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "A confirmação não corresponde à senha.")]
    [Display(Name = "Confirmar senha")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public sealed class ChangeOtherOperatorPasswordInputModel
{
    [Required]
    public string OperatorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha atual do operador selecionado.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha atual do operador")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a nova senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "A confirmação não corresponde à nova senha.")]
    [Display(Name = "Confirmar nova senha")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public sealed class OperatorListItemViewModel
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
