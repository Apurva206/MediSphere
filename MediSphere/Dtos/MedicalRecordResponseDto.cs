namespace MediSphere.Dtos
{
    public class MedicalRecordResponseDto
    {
        public int RecordId { get; set; }
        public string? Symptoms { get; set; }
        public DateTime ConsultationDate { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? PrescribedTests { get; set; }
        public PatientDto? Patient { get; set; }
        public DoctorDto? Doctor { get; set; }
        public AppointmentResponseDto? Appointment { get; set; }
    }
}
