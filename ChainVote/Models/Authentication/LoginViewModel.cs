using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.Authentication
{
    public class LoginViewModel
    {
        [Display(Name = "Student ID or Email")]
        [Required(ErrorMessage = "Email or Student ID is required")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
