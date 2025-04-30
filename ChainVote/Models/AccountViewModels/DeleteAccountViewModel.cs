using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.AccountViewModels
{
    public class DeleteAccountViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
