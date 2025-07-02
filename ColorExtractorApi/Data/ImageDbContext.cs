using Microsoft.EntityFrameworkCore;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Data
{
    public class ImageDbContext : DbContext
    {
        public ImageDbContext(DbContextOptions<ImageDbContext> options)
            : base(options) { }

        public DbSet<ImageColor> ImageColors { get; set; } = null!;
    }
}