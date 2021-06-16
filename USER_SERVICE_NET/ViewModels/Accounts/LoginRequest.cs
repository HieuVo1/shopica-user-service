using System.ComponentModel.DataAnnotations;

namespace USER_SERVICE_NET.ViewModels.Accounts
{
    public class LoginRequest
    {
        [EmailAddress, Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
