namespace ColorExtractorApi.Services
{
    public interface IUserService
    {
        Task<bool> UpdateUserInfoAsync(int userId, string name, string surname, string email);
        Task<bool> DeleteUserAsync(int userId, string password);
    }
}