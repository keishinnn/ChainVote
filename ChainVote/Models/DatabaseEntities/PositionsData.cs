using System.ComponentModel.DataAnnotations;

namespace ChainVote.Models.DatabaseEntities
{
    public class PositionsData
    {
        [Key]
        public int Id { get; set; }

        public int EventId { get; set; } // Foreign key to EventsData
        public string PositionName { get; set; }

        public EventsData Event { get; set; }
    }
}
