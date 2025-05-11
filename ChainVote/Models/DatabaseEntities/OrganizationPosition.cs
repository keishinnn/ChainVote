using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainVote.Models.DatabaseEntities
{
    public class OrganizationPosition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public OrganizationsData Organization { get; set; }

        public int? CandidateId { get; set; }

        [ForeignKey("CandidateId")]
        public CandidatesData Candidate { get; set; }
    }
}
