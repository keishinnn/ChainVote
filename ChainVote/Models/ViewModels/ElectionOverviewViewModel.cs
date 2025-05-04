using ChainVote.Models.DatabaseEntities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChainVote.Models.ViewModels
{
    public class ElectionOverviewViewModel
    {
        public EventsData NewEvent { get; set; } = new EventsData(); // For Create Election Form
        public string SelectedElectionType { get; set; }

        public List<ElectionSummary> AwaitingElections { get; set; } = new();
        public List<ElectionSummary> InProgressElections { get; set; } = new();
        public List<ElectionSummary> CompletedElections { get; set; } = new();

        [BindNever]
        public List<SelectListItem> ElectionTypes { get; set; }

        public ElectionOverviewViewModel()
        {
            // Use the enum to populate dropdown options
            ElectionTypes = Enum.GetValues(typeof(ElectionType))
                .Cast<ElectionType>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(), // Use enum name as value
                    Text = e switch
                    {
                        ElectionType.CampusGovernment => "CSG/USG",
                        ElectionType.ClassOfficer => "Class Officer",
                        _ => e.ToString()
                    }
                }).ToList();
        }
    }

    public class ElectionSummary
    {
        public EventsData Event { get; set; }
        public int TotalVoters { get; set; }
        public int TotalVoted { get; set; }
    }
}
