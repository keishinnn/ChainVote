using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainVote.Models.DatabaseEntities
{
    public class Votes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VoterId { get; set; } // Foreign key to Voters (if you're tracking individual voters)

        [Required]
        public int CandidateId { get; set; } // Foreign key to CandidatesData

        [Required]
        public int EventId { get; set; } // Foreign key to EventsData

        public DateTime VoteDate { get; set; } // Track when the vote was cast (optional)

        // Optional: Add field to track if a vote is valid
        public bool IsValidVote { get; set; } = true;

        // Navigation properties
        public CandidatesData Candidate { get; set; }
        public EventsData Event { get; set; }
    }
}
