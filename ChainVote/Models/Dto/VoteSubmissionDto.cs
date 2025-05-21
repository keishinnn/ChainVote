namespace ChainVote.Models.Dto
{
    public class VoteSubmissionDto
    {
        public int EventId { get; set; }
        public List<VoteItem> Votes { get; set; }
    }

    public class VoteItem
    {
        public int PositionId { get; set; }
        public int CandidateId { get; set; }
    }
}
