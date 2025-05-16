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

        public int? PositionId { get; set; }  // Foreign Key

        [ForeignKey("PositionId")]
        public OrganizationPosition Position { get; set; }

    }

}