using System.ComponentModel.DataAnnotations;

namespace DotNetIdentity.ViewModels;

public class PasswordRecoveryViewModel
{
    [Required(ErrorMessage = "El campo email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "El campo contraseña es obligatorio")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; }

    [Required(ErrorMessage = "El campo confirmar contraseña es obligatorio")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
}