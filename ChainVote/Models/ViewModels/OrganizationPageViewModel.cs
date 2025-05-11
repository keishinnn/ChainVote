using ChainVote.Models.DatabaseEntities;

namespace ChainVote.Models.ViewModels
{
    public class OrganizationPageViewModel
    {
        public List<OrganizationOverviewViewModel> Organizations { get; set; } = new();
        public List<EventsData> Events { get; set; } = new();
    }
}