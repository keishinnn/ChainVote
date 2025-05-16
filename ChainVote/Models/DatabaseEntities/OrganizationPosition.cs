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

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public OrganizationsData Organization { get; set; }

        public ICollection<CandidatesData> Candidates { get; set; } = new List<CandidatesData>();

    }
}