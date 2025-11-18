using System.ComponentModel.DataAnnotations;

namespace medical_record_system_backend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(16)]
        public string Gender { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? AvatarPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
