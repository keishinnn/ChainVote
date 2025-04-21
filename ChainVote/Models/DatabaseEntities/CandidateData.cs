using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
    public class CandidateData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Position { get; set; }
        public string PartyList { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
