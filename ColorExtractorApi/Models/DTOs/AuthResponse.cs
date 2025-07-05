namespace ColorExtractorApi.Models.DTOs
{
    public class AuthResponse
    {
        public bool Success { get; internal set; } // Was logi/register successful or not
        public string Message { get; internal set; } = string.Empty; // Success/error message
        public string Token { get; set; } = string.Empty; // The access token (JWT)
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; } // Access token expiry
        public string RefreshToken { get; set; } = string.Empty; // Refresh token
        public DateTime RefreshTokenExpiresAt { get; set; } // Refresh token expiry
        public UserDto? User { get; set; } // Return user details
    }
}