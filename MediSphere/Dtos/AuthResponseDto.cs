namespace MediSphere.Dtos
{
    public class AuthResponseDto
    {
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public int UserId { get; set; } 
        
    }
}
