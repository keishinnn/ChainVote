namespace ChainVote.Models.Dto
{
    public class CandidateDto
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; }
        public string OrganizationName { get; set; }
    }

    public class PositionDto
    {
        public int PositionId { get; set; }
        public string PositionTitle { get; set; }
        public string OrganizationName { get; set; }
        public List<CandidateDto> Candidates { get; set; } = new();
    }

}