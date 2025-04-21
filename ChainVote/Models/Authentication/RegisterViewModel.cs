using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.Authentication
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Student ID must be greater than 5 characters.")]
        public string StudentId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } = "Voter";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string YearLevel { get; set; }

        [Required]
        public string Section { get; set; }
    }
}
