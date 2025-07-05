using ColorExtractorApi.Models;
using ColorExtractorApi.Models.DTOs;

namespace ColorExtractorApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest model);
        Task<AuthResponse> LoginAsync(LoginRequest model);
        Task<UserDto?> ValidateTokenAsync(string token);
        Task<AuthResponse> RefreshTokenAsync(string refreshTokenStr);
    }
}