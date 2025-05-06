using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
    public enum ElectionType
    {
        ClassOfficer,
        CampusGovernment
    }
    public enum ElectionStatus
    {
        Awaiting,
        InProgress,
        Completed
    }

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
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date and time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public ElectionStatus Status { get; set; }

        public ElectionType ElectionType { get; set; }

        [MaxLength(50)]
        public string? AllowedYearLevels { get; set; }

        [MaxLength(100)]
        public string? AllowedSections { get; set; }

        [MaxLength(100)]
        public string? AllowedCourses { get; set; }

        public ICollection<OrganizationsData> Organizations { get; set; }

    }
}
