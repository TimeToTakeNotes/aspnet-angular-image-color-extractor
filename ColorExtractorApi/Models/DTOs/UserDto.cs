using System.ComponentModel.DataAnnotations;

namespace ColorExtractorApi.Models.DTOs
{
    public class UserDto
    {
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

        // Constructor: takes User (needs instructions on how to turn User obj into UserDto obj)
        public UserDto(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Surname = user.Surname;
            Email = user.Email;
        }
    }

    public class UserUpdateRequestDto
    {
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
    }

    public class UserDeleteRequestDto // Used to verify user identity before deletion
    {
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}