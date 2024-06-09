using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace Gallery.Models
{
    public class ApplicationUser:IdentityUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Username { get; set; } = string.Empty;

        public byte[] ?PasswordHash { get; set; }
        public byte[] ?PasswordSalt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        [Required]
        public string ?Role { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }


    }
}
