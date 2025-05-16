using ChainVote.Models.DatabaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChainVote.Models.Dto
{
    public class OrganizationWithPositionsDto
    {
        public string Name { get; set; }
        public string ElectionType { get; set; }
        public List<string> Positions { get; set; }
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public EventsData Event { get; set; }
    }

}
