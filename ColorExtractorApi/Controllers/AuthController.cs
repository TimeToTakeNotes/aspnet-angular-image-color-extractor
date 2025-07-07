using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ColorExtractorApi.Controllers.Helpers;
using ColorExtractorApi.Services;
using ColorExtractorApi.Models.DTOs;

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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);

            if (!response.Success)
            {
                return BadRequest(new { response.Message });
            }

            CookieHelper.SetAuthCookies(
                Response,
                response.Token!,
                response.RefreshToken!,
                response.ExpiresAt,
                response.RefreshTokenExpiresAt
            );

            return Ok(new { response.Message, response.User });
        }

        // Authenticates an existing user and returns a token if credentials are valid.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
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

            // Set HttpOnly cookies here
            CookieHelper.SetAuthCookies(
                Response,
                response.Token!,
                response.RefreshToken!,
                response.ExpiresAt,
                response.RefreshTokenExpiresAt
            );


            return Ok(new { response.Message, response.User });
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
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(new { Message = "Refresh token missing." });
            }

            var response = await _authService.RefreshTokenAsync(refreshToken);

            if (!response.Success)
            {
                return Unauthorized(new { response.Message });
            }

            CookieHelper.SetAuthCookies(
                Response,
                response.Token!,
                response.RefreshToken!,
                response.ExpiresAt,
                response.RefreshTokenExpiresAt
            );

            return Ok(new { response.Message });
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Ok(new { Message = "Logged out successfully." });
        }
    }
}