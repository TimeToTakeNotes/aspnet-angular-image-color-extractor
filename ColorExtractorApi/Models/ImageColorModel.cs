namespace ColorExtractorApi.Models
{
    public class ImageColor
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty; // Store the img path to uploads folder
        public string ThumbnailPath { get; set; } = string.Empty; // Store the thumbnail path to uploads/thumbnails folder
        public string? HexColor { get; set; }
    }
}