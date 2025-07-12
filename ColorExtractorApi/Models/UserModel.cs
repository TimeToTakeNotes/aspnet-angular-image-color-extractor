using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ColorExtractorApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonIgnore] // Ignore this property during serialization
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public List<RefreshToken> RefreshTokens { get; set; } = new();

    }
}
