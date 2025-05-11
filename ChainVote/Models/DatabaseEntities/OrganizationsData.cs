using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainVote.Models.DatabaseEntities
{
    public class OrganizationsData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public int? EventId { get; set; }

        [ForeignKey("EventId")]
        public EventsData? Event { get; set; }

        public ICollection<CandidatesData> Candidates { get; set; } = new List<CandidatesData>();
        public ICollection<OrganizationPosition> Positions { get; set; } = new List<OrganizationPosition>();
    }
}
