using ColorExtractorApi.Models.DTOs;

namespace ColorExtractorApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest model);
        Task<AuthResponse> LoginAsync(LoginRequest model);
        Task<AuthResponse> RefreshTokenAsync(string refreshTokenStr);
        Task<UserDto?> GetUserByIdAsync(int userId);

    }
}