namespace MediSphere.Dtos
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public char Gender { get; set; } 
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? MedicalHistory { get; set; }
    }
}
