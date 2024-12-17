using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MediSphere.Models
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }

        public string? Symptoms { get; set; }
        public DateTime? ConsultationDate { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? PrescribedTests { get; set; }

        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public Appointment? Appointment { get; set; }
        public ICollection<Prescription>? Prescriptions { get; set; }

       public bool IsDeleted { get; set; } = false;
    }
}
