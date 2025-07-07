using System.ComponentModel.DataAnnotations;

namespace ColorExtractorApi.Models
{
    public class ImageListItemDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [MaxLength(7)]
        public string? HexColor { get; set; }
    }

    public class ImageDetailDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(7)]
        public string? HexColor { get; set; }
    }
}