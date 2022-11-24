using System.ComponentModel.DataAnnotations;

namespace DotNetIdentity.ViewModels;

public class ForgottenPasswordViewModel
{
    [Required(ErrorMessage = "El campo email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; }
}