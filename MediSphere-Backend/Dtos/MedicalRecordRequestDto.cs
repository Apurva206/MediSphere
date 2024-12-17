namespace MediSphere.Dtos
{
    public class MedicalRecordRequestDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
        public string? Symptoms { get; set; }
        public DateTime ConsultationDate { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? PrescribedTests { get; set; }
    }
}
