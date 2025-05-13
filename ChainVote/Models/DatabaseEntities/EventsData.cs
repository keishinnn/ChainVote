using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
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

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public ElectionStatus Status { get; set; }

        [MaxLength(50)]
        public string? AllowedYearLevels { get; set; }

        [MaxLength(100)]
        public string? AllowedSections { get; set; }

        [MaxLength(100)]
        public string? AllowedCourses { get; set; }

        public ICollection<OrganizationsData> Organizations { get; set; } = new List<OrganizationsData>();
        public ICollection<OrganizationPosition> Positions { get; set; } = new List<OrganizationPosition>();
    }
}
