using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Frontend.DTOs.Keycloak
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Parola zorunludur.")]
        public string? Password { get; set; }
    }
}
