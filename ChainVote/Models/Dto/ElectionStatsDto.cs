namespace ChainVote.Models.Dto
{
    public class ElectionStatsDto
    {
        public int TotalVoters { get; set; }
        public int VotesCast { get; set; }
        public double VoterTurnoutPercent { get; set; }
        public List<PositionStatsDto> Positions { get; set; }
    }

    public class PositionStatsDto
    {
        public string PositionTitle { get; set; }
        public List<CandidateVoteDto> Candidates { get; set; }
    }

    public class CandidateVoteDto
    {
        public string CandidateName { get; set; }
        public int VoteCount { get; set; }
    }

}
