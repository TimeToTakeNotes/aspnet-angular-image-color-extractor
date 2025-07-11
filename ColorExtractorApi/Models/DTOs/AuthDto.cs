using System;
using System.ComponentModel.DataAnnotations;

namespace ColorExtractorApi.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool Success { get; internal set; } // Was login/register successful or not

        public string Message { get; internal set; } = string.Empty; // Success/error message

        public string Token { get; set; } = string.Empty; // The access token (JWT)

        public string TokenType { get; set; } = "Bearer";

        public DateTime ExpiresAt { get; set; } // Access token expiry

        public string RefreshToken { get; set; } = string.Empty; // Refresh token

        public DateTime RefreshTokenExpiresAt { get; set; } // Refresh token expiry

        public UserDto? User { get; set; } // Return user details
    }
}