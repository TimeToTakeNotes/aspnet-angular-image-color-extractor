using Microsoft.EntityFrameworkCore;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
    }
}