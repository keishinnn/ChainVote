using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
    public class CandidatesData
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Candidate name is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        [MaxLength(50)]
        public string Position { get; set; }

        [MaxLength(100)]
        public string PartyList { get; set; }

        [Required(ErrorMessage = "Candidate email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(100)]
        public string Email { get; set; }

        // Optional: Add an image URL for candidate photos (if needed)
        public string CandidateImage { get; set; }
    }
}
