using ChainVote.Models.DatabaseEntities;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ChainVote.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string StudentId { get; set; }
        public string YearLevel { get; set; }
        public string Section { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Course { get; set; }
        public bool HasVoted { get; set; }

        // ✅ Change to one-to-one
        public CandidatesData Candidate { get; set; }
    }
}
