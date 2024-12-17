using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MediSphere.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        [ForeignKey("MedicalRecord")]
        public int RecordId { get; set; }

        [Required]
        public string? MedicineName { get; set; }

        public string? Dosage { get; set; }
        public int Frequency { get; set; }
        public string? FoodInstructions { get; set; }

        // Navigation properties
        public MedicalRecord? MedicalRecord { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
