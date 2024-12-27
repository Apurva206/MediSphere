using MediSphere.Repository;
using MediSphere.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediSphere.Services;

namespace MediSphere.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IEmailService _emailService;
        public UserController(IUserServices userServices, IEmailService emailService)
        {
            _userServices = userServices;
            _emailService = emailService;
        }

        // POST: api/User/register
        // Public endpoint: No authorization required
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userServices.RegisterUserAsync(registerDto.Username, registerDto.Password, registerDto.Role, registerDto.Email);
            if (!result)
                return BadRequest("Registration failed. The username might already be taken.");

            // Send the email after successful registration
            var emailSubject = "Welcome to MediSphere!";
            var emailBody = $"Hello {registerDto.Username},\n\nYour registration was successful!";
            var receiverEmail = registerDto.Email;
            _emailService.SendEmail(new List<string> { receiverEmail }, emailSubject, emailBody);

            return Ok("User registered successfully.");
        }

        [AllowAnonymous]
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] string emailAddress)
        {
            var user = await _userServices.ForgotPassword(emailAddress);
            var userData = $"Dear User your data is as follows:\nEmail:{user.Email.ToString()}\nUsername:{user.Username.ToString()}\n" +
                $"Password:{user.Password.ToString()}\nRole:{user.Role.ToString()}\nThanks and Regards\nTeam MediSphere";
            _emailService.SendEmail([emailAddress], "Forgot Password Credentials", userData);

            return Ok("Login Credential sent!");
        }

        // POST: api/User/login
        // Public endpoint: No authorization required
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Authenticate user and get the token and role
            var authResponse = await _userServices.AuthenticateAsync(loginDto.Username, loginDto.Password, loginDto.Email);
            if (authResponse == null)
                return Unauthorized("Invalid username or password.");

            // Return AuthResponseDto with token, role, and expiry
            return Ok(new
            {
                username = loginDto.Username,
                role = authResponse.Role,  // Assuming you return the role in the response
                email = loginDto.Email,
                token = authResponse.Token,  // The actual JWT token
                tokenExpiry = authResponse.TokenExpiry  // The token expiry time
            });
        }

        // GET: api/User/auth
        // Example of a protected endpoint requiring authentication
        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("auth")]
        public IActionResult TestAuth()
        {
            return Ok("Authentication successful!");
        }
    }
}
