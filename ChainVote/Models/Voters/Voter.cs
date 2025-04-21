namespace ChainVote.Models
{
    public class Voter
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string Email { get; set; }
        public string Course { get; set; }
        public string YearLevel { get; set; }
        public bool HasVoted { get; set; }
    }
}