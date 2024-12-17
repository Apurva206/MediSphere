using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MediSphere.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string? Status { get; set; }
        public string? Symptoms { get; set; }
        public string? ConsultationNotes { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }

        // Soft deletion property
        public bool IsDeleted { get; set; } = false;
    }
}
