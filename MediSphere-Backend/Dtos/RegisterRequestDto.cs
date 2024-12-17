using System.ComponentModel.DataAnnotations;

namespace MediSphere.Dtos
{
    public class RegisterRequestDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Role { get; set; }

        [Required]
        public string? Email { get; set; }
    }
}
