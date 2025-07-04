using Microsoft.AspNetCore.Identity;
using ColorExtractorApi.Helpers;
using ColorExtractorApi.Models;
using ColorExtractorApi.Models.DTOs;
using ColorExtractorApi.Repository;

namespace ColorExtractorApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtUtils _jwtUtils;

        public AuthService(IUserRepository userRepository, JwtUtils jwtUtils)
        {
            _userRepository = userRepository;
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

            var (token, expiresAt) = _jwtUtils.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                TokenType = "Bearer", // So client knows what scheme to use
                ExpiresAt = expiresAt, // Match expiry in JWTUtils.GenerateToken()
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

            var (token, expiresAt) = _jwtUtils.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                TokenType = "Bearer", // So client knows what scheme to use
                ExpiresAt = expiresAt,
                User = new UserDto(user)
            };
        }

        public async Task<UserDto?> ValidateTokenAsync(string token)
        {
            var userId = _jwtUtils.ValidateToken(token);
            if (userId == null) return null;

            var user = await _userRepository.GetByIdAsync(userId.Value);
            return user != null ? new UserDto(user) : null;
        }
    }
}
