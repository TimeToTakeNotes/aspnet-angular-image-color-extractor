using Microsoft.AspNetCore.Mvc;
using ColorExtractorApi.Models.DTOs;
using ColorExtractorApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("nameid");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { Message = "Invalid token or user ID claim missing." });
            }

            var userDto = await _authService.GetUserByIdAsync(userId);
            if (userDto == null)
            {
                return Unauthorized(new { Message = "User not found." });
            }

            return Ok(userDto);
        }

        // Accepts a refresh token and returns new access + refresh tokens if valid.
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new { Message = "Invalid refresh token data." });
            }

            var response = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (!response.Success)
            {
                return Unauthorized(new { response.Message });
            }

            return Ok(response);
        }
    }
}