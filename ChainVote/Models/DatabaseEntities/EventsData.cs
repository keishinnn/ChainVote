using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ChainVote.Models.DatabaseEntities
{
    public class EventsData
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [MaxLength(100)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Creator/Admin email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Start date and time is required.")]
        [DataType(DataType.DateTime)]  // Use DataType.DateTime for date and time
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date and time is required.")]
        [DataType(DataType.DateTime)]  // Use DataType.DateTime for date and time
        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(20)]
        public string Status { get; set; } = "Awaiting";

        [Required(ErrorMessage = "The Organizations field is required.")]
        [MaxLength(100)]
        public string Organizations { get; set; } = "DefaultOrganization";

        [Required(ErrorMessage = "The Election Type is required.")]
        public string ElectionType { get; set; }
    }
}
