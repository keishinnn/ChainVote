using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
    public class AdminAccount
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; } = "Admin"; // default to Admin

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
