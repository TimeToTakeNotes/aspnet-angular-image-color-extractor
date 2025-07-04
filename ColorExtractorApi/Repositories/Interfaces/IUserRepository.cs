using ColorExtractorApi.Models;

namespace ColorExtractorApi.Repository
{
    // Interface defines 'what' functions any user repo must provide:
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task CreateAsync (User user);
        Task<bool> EmailExistsAsync(string email);
    }
}