// File: Dtos/PositionWithCandidateDto.cs

namespace ChainVote.Models.Dtos
{
    public class PositionWithCandidateDto
    {
        public string PositionName { get; set; } // title
        public CandidateDto? AssignedCandidate { get; set; }
    }

    public class CandidateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
