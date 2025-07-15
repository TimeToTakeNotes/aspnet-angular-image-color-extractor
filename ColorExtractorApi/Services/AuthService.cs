using Microsoft.AspNetCore.Identity;
using ColorExtractorApi.Models;
using ColorExtractorApi.Models.DTOs;
using ColorExtractorApi.Repository;
using ColorExtractorApi.Services.Helpers;
using ColorExtractorApi.Services.Interfaces;


namespace ColorExtractorApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILoggerService _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshRepository;
        private readonly JwtUtils _jwtUtils;

        public AuthService(ILoggerService logger, IUserRepository userRepository, IRefreshTokenRepository refreshRepository, JwtUtils jwtUtils)
        {
            _logger = logger;
            _userRepository = userRepository;
            _refreshRepository = refreshRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto model)
        {
            _logger.LogInfo($"[Register] Attempt for email: {model.Email}");

            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                _logger.LogWarn($"[Register] Failed - Email already registered: {model.Email}");
                return new AuthResponseDto { Success = false, Message = "Email already registered." };
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

            _logger.LogInfo($"[Register] Success - User created: {model.Email}, user id: {user.Id}");

            // Generate all tokens:
            var (jwtToken, jwtExpiresAt, refreshToken) = GenerateTokensForUser(user);
            await _refreshRepository.AddRefreshTokenAsync(refreshToken);
            _logger.LogInfo($"[Register] Tokens generated for user: {user.Email}");

            return new AuthResponseDto
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

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto model)
        {
            _logger.LogInfo($"[Login] Attempt for email: {model.Email}");

            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarn($"[Login] Failed - User not found: {model.Email}");
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarn($"[Login] Failed - Invalid password for: {model.Email}");
                return new AuthResponseDto { Success = false, Message = "Invalid credentials." };
            }

            _logger.LogInfo($"[Login] Success - {model.Email}");

            // Generate all tokens:
            var (jwtToken, jwtExpiresAt, refreshToken) = GenerateTokensForUser(user);
            await _refreshRepository.AddRefreshTokenAsync(refreshToken);

            _logger.LogInfo($"[Login] Tokens generated for user ID: {user.Id}, email: {user.Email}");

            return new AuthResponseDto
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

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshTokenStr)
        {
            _logger.LogInfo($"[Token Refresh] Attempted refresh for token: {refreshTokenStr.Substring(0, 8)}...");

            var refreshToken = await _refreshRepository.GetRefreshTokenAsync(refreshTokenStr);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarn($"[Token Refresh] Failed - Token not found.");
                return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token." };
            }

            if (refreshToken.IsRevoked)
            {
                _logger.LogWarn($"[Token Refresh] Failed - Token already revoked.");
                return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token." };
            }

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarn($"[Token Refresh] Failed - Token expired.");
                return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token." };
            }

            // Revoke old refresh token
            refreshToken.IsRevoked = true;
            await _refreshRepository.UpdateRefreshTokenAsync(refreshToken);
            _logger.LogInfo($"[Token Refresh] Old token revoked.");

            // Generate new tokens
            var user = refreshToken.User;
            var (newJwt, jwtExpiresAt, newRefreshToken) = GenerateTokensForUser(user);
            await _refreshRepository.AddRefreshTokenAsync(newRefreshToken);
            _logger.LogInfo($"[Token Refresh] New tokens issued for user ID: {user.Id}, email: {user.Email}");

            return new AuthResponseDto
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
            _logger.LogInfo($"[GetUserById] Lookup for userId: {userId}");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarn($"[GetUserById] No user found with ID: {userId}");
                return null;
            }
            
            _logger.LogInfo($"[GetUserById] User found: {user.Email}");
            return new UserDto(user);
        }


        // Helper method: Centralizes token creation logic:
        private (string jwt, DateTime jwtExpires, RefreshToken refreshToken) GenerateTokensForUser(User user)
        {
            var (jwt, jwtExpires) = _jwtUtils.GenerateToken(user);
            var refreshToken = RefreshUtils.GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            _logger.LogDebug($"[GenerateTokens] Tokens generated for userId: {user.Id}, email: {user.Email}");
            
            return (jwt, jwtExpires, refreshToken);
        }
    }
}