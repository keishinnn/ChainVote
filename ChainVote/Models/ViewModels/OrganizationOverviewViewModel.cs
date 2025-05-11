

namespace ChainVote.Models.ViewModels
{
    public class OrganizationOverviewViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public int? EventId { get; set; }
        public string? EventName { get; set; }  // For display

        public List<string> Positions { get; set; } = new();
    }
}
