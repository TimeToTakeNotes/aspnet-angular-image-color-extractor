using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ColorExtractorApi.Data;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ColorExtractorContext _context;

        // Contructor: inject ColorExtractContext (EF Core DbContext) so we can perform db operations
        public UserRepository(ColorExtractorContext context)
        {
            _context = context;
        }

        // Gets user by email address
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Gets user by their ID
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Adds a new user to db
        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Checks if user with specified email exists
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UpdateUserInfoAsync(int userId, string name, string surname, string email)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Check if there's any change
            if (user.Name == name && user.Surname == surname && user.Email == email)
                return false;

            // Check if email is changed and if it's already in use by another user
            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                bool emailExists = await _context.Users
                    .AnyAsync(u => u.Email == email && u.Id != userId);
                if (emailExists)
                    throw new InvalidOperationException("Email is already in use by another user.");
            }

            // Apply updates
            user.Name = name;
            user.Surname = surname;
            user.Email = email;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePasswordAsync(User user, string newPassword)
        {
            var dbUser = await _context.Users.FindAsync(user.Id); // Find user by ID
            if (dbUser == null) // user not found  
                return false;

            dbUser.PasswordHash = new PasswordHasher<User>().HashPassword(dbUser, newPassword); // Hash the new password
            dbUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId); // Find user by ID
            if (user == null) // user not found 
                return false;

            _context.Users.Remove(user); // Remove user along with refresh tokens and images related to this user via cascade delete
            await _context.SaveChangesAsync();
            return true;
        }
    }
}