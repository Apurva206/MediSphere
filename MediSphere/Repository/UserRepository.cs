using MediSphere.Dtos;
using MediSphere.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediSphere.Repository
{
    public class UserRepository : IUserServices
    {
        private readonly MediSphereDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserRepository(MediSphereDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Fetch user by username
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users
                                   .FirstOrDefaultAsync(u => u.Username == username);
        }

        // Validate username and password
        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _dbContext.Users
                                       .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            return user != null;
        }

        // Register a new user
        public async Task<bool> RegisterUserAsync(string username, string password, string role, string email)
        {
            // Check if the username already exists
            var existingUser = await _dbContext.Users
                                               .FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null) return false; // Username already taken

            // Create a new user
            var user = new User
            {
                Username = username,
                Password = password,
                Role = role,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            // Assign DoctorId or PatientId based on the role
            //if (role == "Doctor")
            //{
            //    user.DoctorId = GenerateDoctorId(); // Assign DoctorId for Doctor role
            //}
            //else if (role == "Patient")
            //{
            //    user.PatientId = GeneratePatientId(); // Assign PatientId for Patient role
            //}

            await _dbContext.Users.AddAsync(user);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<User> ForgotPassword(string emailAddress)
        {
           var user =  await _dbContext.Users.FirstAsync(u=>u.Email==emailAddress);
            return user;
        }
        //private int GenerateDoctorId()
        //{
        //    // Logic to generate a unique DoctorId
        //    return new Random().Next(0, 10); // Example random DoctorId generator
        //}

        //private int GeneratePatientId()
        //{
        //    // Logic to generate a unique PatientId
        //    return new Random().Next(0, 10); // Example random PatientId generator
        //}

        // Authenticate a user and generate JWT token
        public async Task<AuthResponseDto> AuthenticateAsync(string username, string password, string email)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.Email == email);

            if (user == null) return null;

            // Generate JWT token
            var token = GenerateJwtToken(user);
            var tokenExpiry = DateTime.UtcNow.AddHours(2); // Set token expiry time

            return new AuthResponseDto
            {
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                Token = token,
                TokenExpiry = tokenExpiry,
                UserId = user.UserId
            };
        
        }

        // Helper method to generate the JWT token
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
           

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // Secret key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Expiry time (2 hours)
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Return the JWT token as a string
        }
    }
}
