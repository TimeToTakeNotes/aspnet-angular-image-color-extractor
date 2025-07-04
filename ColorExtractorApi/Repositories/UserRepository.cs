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
    }
}