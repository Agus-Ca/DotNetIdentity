using System.ComponentModel.DataAnnotations;

namespace DotNetIdentity.ViewModels;

public class ExternalAccessConfrimationViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [EmailAddress]
    public string Name { get; set; }
}