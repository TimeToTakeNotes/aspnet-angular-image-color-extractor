// Design-time factory for creating the UserDbContext during migrations -> Req because multiple DbContext classes are now used

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ColorExtractorApi.Data;

namespace ColorExtractorApi.Data
{
    public class DesignTimeUserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

            var serverName = Environment.GetEnvironmentVariable("SERVER_NAME") ?? "localhost"; // SERVER_NAME can be set in env to override the default SQL Server instance name (e.g. "localhost", "(localdb)\\mssqllocaldb")
            optionsBuilder.UseSqlServer($"Server={serverName};Database=ColorExtractorDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new UserDbContext(optionsBuilder.Options);
        }
    }
}