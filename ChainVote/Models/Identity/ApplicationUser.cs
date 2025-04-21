using Microsoft.AspNetCore.Identity;

namespace ChainVote.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string StudentId { get; set; }
        public string YearLevel { get; set; }
        public string Section { get; set; }
    }
}
