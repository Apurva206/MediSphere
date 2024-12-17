namespace MediSphere.Dtos
{
    public class PrecriptionDto
    {
        public int PrescriptionId { get; set; }
        public int RecordId { get; set; }  
        public string? MedicineName { get; set; }  
        public string? Dosage { get; set; }  
        public int? Frequency { get; set; }  
        public string? FoodInstructions { get; set; }  
    }
}
