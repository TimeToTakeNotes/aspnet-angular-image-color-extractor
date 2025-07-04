using Microsoft.AspNetCore.Mvc;
using ColorExtractorApi.Models.DTOs;
using ColorExtractorApi.Services;

namespace ColorExtractorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Registers a new user and returns an auth token if successful.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            var response = await _authService.RegisterAsync(request);

            if (!response.Success)
            {
                return BadRequest(new { response.Message });
            }

            return Ok(response);
        }

        // Authenticates an existing user and returns a token if credentials are valid.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }

            var response = await _authService.LoginAsync(request);

            if (!response.Success)
            {
                return Unauthorized(new { response.Message });
            }

            return Ok(response);
        }

        /// Validates a token and returns user info if valid.
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Missing token." });
            }

            var userDto = await _authService.ValidateTokenAsync(token);
            if (userDto == null)
            {
                return Unauthorized(new { Message = "Invalid or expired token." });
            }

            return Ok(userDto);
        }
    }
}