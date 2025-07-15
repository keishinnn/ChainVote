using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.Authentication
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Student ID or Email is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
