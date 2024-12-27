using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MediSphere.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public string? FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public char Gender { get; set; }

        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? MedicalHistory { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
        //public bool IsDeleted { get; set; } = false;
    }
}
