using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChainVote.Models.Identity;

namespace ChainVote.Models.DatabaseEntities
{
    public class VoteRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VoterId { get; set; } // FK to ApplicationUser

        [ForeignKey("VoterId")]
        public ApplicationUser Voter { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [ForeignKey("CandidateId")]
        public CandidatesData Candidate { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public EventsData Event { get; set; }

        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}
