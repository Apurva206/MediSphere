using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MediSphere.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? Specialty { get; set; }

        public int Experience { get; set; }
        public string? Qualification { get; set; }
        public string? Designation { get; set; }
        public string? ContactNumber { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
       // public bool IsDeleted { get; set; } = false;
    }
}
