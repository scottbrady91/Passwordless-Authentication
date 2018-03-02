using System.ComponentModel.DataAnnotations;

namespace ScottBrady91.PasswordlessAuthentication.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}