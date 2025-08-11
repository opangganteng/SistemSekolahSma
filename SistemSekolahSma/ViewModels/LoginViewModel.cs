using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username harus diisi")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password harus diisi")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pilih role login")]
        public string RoleType { get; set; }

        public bool RememberMe { get; set; }
    }
}