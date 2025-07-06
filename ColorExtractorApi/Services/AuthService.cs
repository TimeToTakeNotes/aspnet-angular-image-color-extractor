using Microsoft.AspNetCore.Identity;
using ColorExtractorApi.Services.Helpers;
using ColorExtractorApi.Models;
using ColorExtractorApi.Models.DTOs;
using ColorExtractorApi.Repository;

namespace ColorExtractorApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshRepository;
        private readonly JwtUtils _jwtUtils;

        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshRepository, JwtUtils jwtUtils)
        {
            _userRepository = userRepository;
            _refreshRepository = refreshRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest model)
        {
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new AuthResponse { Success = false, Message = "Email already registered." };
            }

            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

            await _userRepository.CreateAsync(user);

            // Generate all tokens:
            var (jwtToken, jwtExpiresAt, refreshToken) = GenerateTokensForUser(user);

            await _refreshRepository.AddRefreshTokenAsync(refreshToken);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful.",
                Token = jwtToken,
                TokenType = "Bearer", // So client knows what scheme to use
                ExpiresAt = jwtExpiresAt, // Match expiry in JWTUtils.GenerateToken()
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                User = new UserDto(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest model)
        {
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "Invalid credentials." };
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return new AuthResponse { Success = false, Message = "Invalid credentials." };
            }

            // Generate all tokens:
            var (jwtToken, jwtExpiresAt, refreshToken) = GenerateTokensForUser(user);

            await _refreshRepository.AddRefreshTokenAsync(refreshToken);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful.",
                Token = jwtToken,
                TokenType = "Bearer", // So client knows what scheme to use
                ExpiresAt = jwtExpiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                User = new UserDto(user)
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshTokenStr)
        {
            var refreshToken = await _refreshRepository.GetRefreshTokenAsync(refreshTokenStr);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return new AuthResponse { Success = false, Message = "Invalid or expired refresh token." };
            }

            // Revoke old refresh token
            refreshToken.IsRevoked = true;
            await _refreshRepository.UpdateRefreshTokenAsync(refreshToken);

            // Generate new tokens
            var user = refreshToken.User;
            var (newJwt, jwtExpiresAt, newRefreshToken) = GenerateTokensForUser(user);

            await _refreshRepository.AddRefreshTokenAsync(newRefreshToken);

            return new AuthResponse
            {
                Success = true,
                Message = "Token refreshed.",
                Token = newJwt,
                TokenType = "Bearer",
                ExpiresAt = jwtExpiresAt,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
                User = new UserDto(user)
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId) // Not really auth, move to a UserService later
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? new UserDto(user) : null;
        }

        
        // Helper method: Centralizes token creation logic:
        private (string jwt, DateTime jwtExpires, RefreshToken refreshToken) GenerateTokensForUser(User user)
        {
            var (jwt, jwtExpires) = _jwtUtils.GenerateToken(user);
            var refreshToken = RefreshUtils.GenerateRefreshToken();
            refreshToken.UserId = user.Id;
            return (jwt, jwtExpires, refreshToken);
        }
    }
}
