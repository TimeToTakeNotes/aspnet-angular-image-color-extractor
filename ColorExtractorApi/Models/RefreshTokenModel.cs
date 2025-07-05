// Entity for storing user linked refresh tokens used to grant new jwt access tokens 

using System.ComponentModel.DataAnnotations;

namespace ColorExtractorApi.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public bool IsRevoked { get; set; }

        // Link token to user
        [Required]
        public int UserId { get; set; } // FK linked to User table/model

        [Required]
        public User User { get; set; } = null!;
    }
}