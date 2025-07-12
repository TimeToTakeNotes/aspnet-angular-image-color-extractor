using System.ComponentModel.DataAnnotations; // Influence how EF maps models to db

namespace ColorExtractorApi.Models
{
    public class ImageColor
    {
        [Key] // Marks PK of entity
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; } = string.Empty; // Store the img path to uploads folder

        [Required]
        public string ThumbnailPath { get; set; } = string.Empty; // Store the thumbnail path to uploads/thumbnails folder

        [MaxLength(7)]
        public string? HexColor { get; set; }

        // Link image to user
        [Required]
        public int UserId { get; set; } // FK linked to User table/model

        [Required]
        public User User { get; set; } = null!;
    }
}