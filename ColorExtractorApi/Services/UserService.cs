using Microsoft.AspNetCore.Identity;
using ColorExtractorApi.Models;
using ColorExtractorApi.Repository;
using ColorExtractorApi.Services.Helpers;
using ColorExtractorApi.Services.Interfaces;

namespace ColorExtractorApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ImageFolderRemover _imageFolderRemover;

        public UserService(IUserRepository userRepository, ImageFolderRemover imageFolderRemover)
        {
            _userRepository = userRepository;
            _imageFolderRemover = imageFolderRemover;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<bool> UpdateUserInfoAsync(int userId, string name, string surname, string email)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Check for changes
            if (user.Name == name && user.Surname == surname && user.Email == email)
                return false;

            // Check for email uniqueness
            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                bool emailExists = await _userRepository.EmailExistsAsync(email);
                if (emailExists && user.Email != email)
                    throw new InvalidOperationException("Email is already in use by another user.");
            }

            return await _userRepository.UpdateUserInfoAsync(userId, name, surname, email);
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword); // Verify the current password before update
            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Incorrect current password.");

            return await _userRepository.UpdatePasswordAsync(user, newPassword);
        }


        public async Task<bool> DeleteUserAsync(int userId, string password)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password); // Verify user with password before account delete
            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Incorrect password.");

            _imageFolderRemover.DeleteImageFolder(userId);
            return await _userRepository.DeleteAsync(userId);
        }
    }
}