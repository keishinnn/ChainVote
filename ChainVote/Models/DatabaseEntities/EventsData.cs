using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
    public class EventsData
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [MaxLength(100)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(20)]
        public string Status { get; set; } // Awaiting, In Progress, Completed

        [Required(ErrorMessage = "Creator/Admin email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(100)]
        public string Email { get; set; } // Creator/Admin email
    }
}
