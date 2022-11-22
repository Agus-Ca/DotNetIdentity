using System.ComponentModel.DataAnnotations;

namespace DotNetIdentity.ViewModels
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El campo fecha de nachimiento es obligatorio")]
        public DateTime? Birthdate { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "El campo email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo contraseña es obligatorio")]
        [StringLength(50, ErrorMessage = "El {0} debe estar entre al menos {2} caracteres de longitud", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El campo confirmar contraseña es obligatorio")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }

        public string Url { get; set; }

        [Required(ErrorMessage = "El campo pais es obligatorio")]
        public string Country { get; set; }

        public int CountryCode { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "El campo estado es obligatorio")]
        public int State { get; set; }
    }
}