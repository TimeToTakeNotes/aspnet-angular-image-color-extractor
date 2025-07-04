using Microsoft.EntityFrameworkCore;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Data
{
    public class ColorExtractorContext : DbContext
    {
        public ColorExtractorContext(DbContextOptions<ColorExtractorContext> options)
            : base(options) { }
        public DbSet<ImageColor> ImageColors { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}