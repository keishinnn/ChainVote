using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class Voters
{
    public int Id { get; set; }

    [ForeignKey("User")]
    public string ApplicationUserId { get; set; } // FK to Identity User

    public ApplicationUser User { get; set; }

    public string StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Course { get; set; }
    public string YearLevel { get; set; }
    public string Section { get; set; }

    public bool HasVoted { get; set; }

    public int EventId { get; set; }

    [ForeignKey("EventId")]
    public EventsData Event { get; set; }
}
