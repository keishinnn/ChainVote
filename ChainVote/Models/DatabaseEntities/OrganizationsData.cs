using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainVote.Models.DatabaseEntities
{
    public class OrganizationsData
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Organization name is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Organization email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(100)]
        public string Email { get; set; }

        // Foreign key to EventsData
        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public EventsData Event { get; set; }
    }
}
