using System.ComponentModel.DataAnnotations;


namespace MediSphere.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? Role { get; set; }

        [Required]
        public string? Email {  get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Doctor>? Doctors { get; set; }
        public ICollection<Patient>? Patients { get; set; }

        // Properties for Error View
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    }
}