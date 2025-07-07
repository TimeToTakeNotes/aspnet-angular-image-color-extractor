using System.ComponentModel.DataAnnotations;

namespace ColorExtractorApi.Models
{
    public class SelectedImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; } = string.Empty; 

        [MaxLength(7)]
        public string? HexColor { get; set; }
    }
}