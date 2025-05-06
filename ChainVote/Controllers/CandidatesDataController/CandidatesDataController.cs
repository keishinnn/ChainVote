using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using Microsoft.AspNetCore.Identity;
using ChainVote.Models.Identity;
using System.Linq;
using System.Threading.Tasks;
using ChainVote.Utilities;

namespace ChainVote.Controllers.CandidatesController
{
    [Authorize(Roles = "Admin")]
    public class CandidatesDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CandidatesDataController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Method to get eligible candidates
        [HttpGet]
        public IActionResult GetCandidates()
        {
            var candidates = _context.CandidatesData
                .Include(c => c.ApplicationUser) // To load the related user data
                .Select(c => new
                {
                    studentId = c.ApplicationUser.StudentId,
                    fullName = c.ApplicationUser.FirstName + " " + c.ApplicationUser.LastName,
                    yearLevel = FormatHelpers.GetYearWithSuffix(c.ApplicationUser.YearLevel),
                    course = c.ApplicationUser.Course,
                    section = FormatHelpers.GetSectionWithYear(c.ApplicationUser.YearLevel, c.ApplicationUser.Section),
                    email = c.ApplicationUser.Email
                })
                .ToList();

            return Json(new { data = candidates });
        }

        // Method to deploy a candidate (already implemented)
        [HttpPost]
        public async Task<IActionResult> DeployCandidate(string studentId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);

            if (user == null)
                return NotFound();

            // Optional: Check if already a candidate
            var exists = await _context.CandidatesData.AnyAsync(c => c.ApplicationUserId == user.Id);
            if (exists)
                return BadRequest("User is already a candidate.");

            var candidate = new CandidatesData
            {
                ApplicationUserId = user.Id
            };

            _context.CandidatesData.Add(candidate);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Method to delete a deployed candidate
        [HttpPost]
        public async Task<IActionResult> DeleteCandidate(string studentId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);

            if (user == null)
                return NotFound();

            var candidate = await _context.CandidatesData
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (candidate == null)
                return NotFound("Candidate not found.");

            _context.CandidatesData.Remove(candidate);
            await _context.SaveChangesAsync();

            return Ok("Candidate removed successfully.");
        }
    }
}
