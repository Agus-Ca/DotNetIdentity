using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DotNetIdentity.ViewModels;

public class AccessViewModel
{
    [Required(ErrorMessage = "El campo email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "El campo contraseña es obligatorio")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; }

    [Display(Name = "Recordar datos")]
    public bool RememberMe { get; set; }
}