using ChainVote.Models.DatabaseEntities;
using ChainVote.Models;

public class Vote
{
    public int Id { get; set; }

    public int VoterId { get; set; }
    public Voter Voter { get; set; }

    public int EventId { get; set; }
    public EventsData Event { get; set; }

    public DateTime VotedAt { get; set; }
}
