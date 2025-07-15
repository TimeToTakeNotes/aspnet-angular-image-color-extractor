using Microsoft.EntityFrameworkCore;
using ColorExtractorApi.Data;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Repository
{
    // Class for interacting with db for img data:
    public class ImageRepository : IImageRepository
    {
        private readonly ColorExtractorContext _context;

        // Contructor: inject ColorExtractContext (EF Core DbContext) so we can perform db operations
        public ImageRepository(ColorExtractorContext context)
        {
            _context = context;
        }

        // Add new img record to db & saves: 
        public async Task AddImageAsync(ImageColor image)
        {
            await _context.ImageColors.AddAsync(image); // Add new img entity to ImageColors table (doesn't save)
            await _context.SaveChangesAsync(); // Saves changes to db
        }

        // Get single img record using imageId and ensure ownership:
        public async Task<ImageColor?> GetImageByImageIdAsync(int id, int userId)
        {
            return await _context.ImageColors
                .FirstOrDefaultAsync(img => img.Id == id && img.UserId == userId);
        }

        // Get all images for a specific user
        public async Task<IEnumerable<ImageColor>> GetImagesByUserIdAsync(int userId)
        {
            return await _context.ImageColors
                .Where(img => img.UserId == userId)
                .ToListAsync();
        }

        // Delete image using imageId
        public async Task<bool> DeleteImageAsync(int imageId, int userId)
        {
            var image = await _context.ImageColors.FirstOrDefaultAsync(i => i.Id == imageId && i.UserId == userId);
            
            if (image == null)
                return false;

            _context.ImageColors.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}