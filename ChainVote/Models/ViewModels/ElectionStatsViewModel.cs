namespace ChainVote.Models.ViewModels
{
    public class ElectionStatsViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public int TotalVoters { get; set; }
        public int VotesCast { get; set; }
        public double VoterTurnoutPercent { get; set; }
        public List<PositionStatsViewModel> Positions { get; set; }
    }

    public class PositionStatsViewModel
    {
        public string PositionTitle { get; set; }
        public List<CandidateStatsViewModel> Candidates { get; set; }
    }

    public class CandidateStatsViewModel
    {
        public string CandidateName { get; set; }
        public int VoteCount { get; set; }
    }
}
