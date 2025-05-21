

namespace ChainVote.Models.ViewModels;
public class VoteRecordViewModel
{
    public string VoterEmail { get; set; }
    public string CandidateName { get; set; }
    public string PositionName { get; set; }
    public string OrganizationName { get; set; }
    public string EventName { get; set; }
    public DateTime VotedAt { get; set; }
}
