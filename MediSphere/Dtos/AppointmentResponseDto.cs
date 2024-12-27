namespace MediSphere.Dtos
{
    public class AppointmentResponseDto //For returning appointment data
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }
        public string? Symptoms { get; set; }
        public string? ConsultationNotes { get; set; }
        public PatientDto? Patient { get; set; }
        public DoctorDto? Doctor { get; set; }
    }
}
