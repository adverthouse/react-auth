using System.ComponentModel.DataAnnotations;

namespace back_end.ViewModels
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; }= "";
    }
}
