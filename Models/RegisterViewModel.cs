using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ime i prezime su obavezni")]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress(ErrorMessage = "Unesite validan email")]
        public string Email { get; set; } = null!;

        [Required, MinLength(6, ErrorMessage = "Lozinka mora imati min. 6 karaktera")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Odaberite tip korisnika")]
        public string Role { get; set; } = null!;
    }
}