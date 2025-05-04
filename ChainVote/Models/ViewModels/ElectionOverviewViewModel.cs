using ChainVote.Models.DatabaseEntities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChainVote.Models.ViewModels
{
    public class ElectionOverviewViewModel
    {
        public EventsData NewEvent { get; set; } = new EventsData(); // For Create Election Form
        public string SelectedElectionType { get; set; }

        public List<ElectionSummary> AwaitingElections { get; set; } = new List<ElectionSummary>();
        public List<ElectionSummary> InProgressElections { get; set; } = new List<ElectionSummary>();
        public List<ElectionSummary> CompletedElections { get; set; } = new List<ElectionSummary>();

        [BindNever]
        public List<SelectListItem> ElectionTypes { get; set; }

        public ElectionOverviewViewModel()
        {
            // Populate ElectionTypes dropdown with values
            ElectionTypes = Enum.GetValues(typeof(ElectionType))
                .Cast<ElectionType>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e == ElectionType.CampusGovernment ? "CSG/USG" : "Class Officer"
                })
                .ToList();
        }
    }

    public class ElectionSummary
    {
        public EventsData Event { get; set; }
        public int TotalVoters { get; set; }
        public int TotalVoted { get; set; }
    }
}
