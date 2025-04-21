using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
    public class EventData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EventName { get; set; }

        public string Status { get; set; } // Awaiting, In Progress, Completed

        [EmailAddress]
        public string Email { get; set; } // Creator/Admin email
    }
}