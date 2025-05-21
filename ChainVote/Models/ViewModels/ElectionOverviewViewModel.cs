using ChainVote.Models.DatabaseEntities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChainVote.Models.ViewModels
{
    public class ElectionOverviewViewModel
    {
        public EventsData NewEvent { get; set; } = new EventsData(); // For Create Election Form
        public List<ElectionSummary> AwaitingElections { get; set; } = new();
        public List<ElectionSummary> InProgressElections { get; set; } = new();
        public List<ElectionSummary> CompletedElections { get; set; } = new();

        public int EventId { get; set; }
    }

    public class ElectionSummary
    {
        public EventsData Event { get; set; }
        public int TotalVoters { get; set; }
        public int TotalVoted { get; set; }
    }
}
