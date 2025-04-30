using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.AccountViewModels
{
    public class EditEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
