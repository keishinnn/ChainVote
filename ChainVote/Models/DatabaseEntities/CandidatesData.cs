using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChainVote.Models.Identity;

namespace ChainVote.Models.DatabaseEntities
{
    public class CandidatesData
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to ApplicationUser
        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        // Nullable FK to Organization
        public int? OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public OrganizationsData Organization { get; set; }
        public ICollection<OrganizationPosition> Positions { get; set; }

    }

}
