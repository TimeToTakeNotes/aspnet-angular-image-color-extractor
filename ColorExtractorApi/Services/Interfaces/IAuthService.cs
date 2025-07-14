using ColorExtractorApi.Models.DTOs;

namespace ColorExtractorApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto model);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshTokenStr);
        Task<UserDto?> GetUserByIdAsync(int userId);

    }
}