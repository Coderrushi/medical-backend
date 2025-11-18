using System.ComponentModel.DataAnnotations;

namespace medical_record_system_backend.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}
