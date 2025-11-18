using System.ComponentModel.DataAnnotations;

namespace medical_record_system_backend.DTOs
{
    public class SignupDto
    {
        [Required] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        [Required, MinLength(6)] public string Password { get; set; }
    }
}
