namespace ColorExtractorApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UpdateUserInfoAsync(int userId, string name, string surname, string email);
        Task<bool> UpdatePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> DeleteUserAsync(int userId, string password);
    }
}