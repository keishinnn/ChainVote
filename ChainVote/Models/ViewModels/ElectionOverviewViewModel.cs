namespace ChainVote.Models.ViewModels
{
    using ChainVote.Models.DatabaseEntities;
    using System.Collections.Generic;

    public class ElectionOverviewViewModel
    {
        public EventsData NewEvent { get; set; } = new EventsData(); // For Create Election Form

        public List<ElectionSummary> AwaitingElections { get; set; } = new List<ElectionSummary>();
        public List<ElectionSummary> InProgressElections { get; set; } = new List<ElectionSummary>();
        public List<ElectionSummary> CompletedElections { get; set; } = new List<ElectionSummary>();
    }


    public class ElectionSummary
    {
        public EventsData Event { get; set; }
        public int TotalVoters { get; set; }
        public int TotalVoted { get; set; }
    }



}
