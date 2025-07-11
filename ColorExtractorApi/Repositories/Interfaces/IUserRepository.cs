using ColorExtractorApi.Models;

namespace ColorExtractorApi.Repository
{
    // Interface defines 'what' functions any user repo must provide:
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task CreateAsync(User user);
        Task<bool> UpdateUserInfoAsync(int userId, string name, string surname, string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> DeleteAsync(int userId);
    }
}