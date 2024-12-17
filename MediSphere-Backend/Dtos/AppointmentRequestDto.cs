namespace MediSphere.Dtos
{
    public class AppointmentRequestDto //When a client schedules an appointment, only the essential input data (e.g., PatientId, DoctorId, AppointmentDate) needs to be sent to the server.
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; } 
        public string? Symptoms { get; set; }
        public string? ConsultationNotes { get; set; }
    }
}
