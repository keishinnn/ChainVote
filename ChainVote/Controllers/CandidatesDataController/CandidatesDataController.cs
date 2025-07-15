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

        // Returns all deployed candidates with their profile and election details
        [HttpGet]
        public IActionResult GetCandidates()
        {
            // Fetch candidates and include related user info
            var candidates = _context.CandidatesData
                .Include(c => c.ApplicationUser)
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

            // Return candidate data in JSON format
            return Json(new { data = candidates });
        }

        // Returns users eligible to be deployed as candidates (non-admins and not yet deployed)
        [HttpGet]
        public IActionResult GetReadyForDeployment()
        {
            // Get IDs of users who are already candidates
            var candidateUserIds = _context.CandidatesData
                .Select(c => c.ApplicationUserId)
                .ToList();

            // Get the role ID for the "Admin" role
            var adminRoleId = _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefault();

            // Get IDs of users who are admins
            var adminUserIds = _context.UserRoles
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .ToList();

            // Select users who are not admins and not already candidates
            var eligibleCandidates = _context.Users
                .Where(u => !adminUserIds.Contains(u.Id) && !candidateUserIds.Contains(u.Id))
                .AsEnumerable()
                .Select(u => new
                {
                    studentId = u.StudentId,
                    fullName = u.FirstName + " " + u.LastName,
                    yearLevel = FormatHelpers.GetYearWithSuffix(u.YearLevel),
                    course = u.Course,
                    section = FormatHelpers.GetSectionWithYear(u.YearLevel, u.Section),
                    email = u.Email
                })
                .ToList();

            // Return eligible users in JSON format
            return Json(new { data = eligibleCandidates });
        }

        // Deploys a candidate to a specified position if not already assigned
        [HttpPost]
        public async Task<IActionResult> DeployCandidate(string studentId, int positionId)
        {
            // Find the user based on student ID
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);
            if (user == null)
                return NotFound();

            // Check if the user is already a candidate
            var exists = await _context.CandidatesData.AnyAsync(c => c.ApplicationUserId == user.Id);
            if (exists)
                return BadRequest("User is already a candidate.");

            // Ensure that the position is not already taken
            var positionTaken = await _context.CandidatesData.AnyAsync(c => c.PositionId == positionId);
            if (positionTaken)
                return BadRequest("This position is already assigned to another candidate.");

            // Add the candidate to the database
            var candidate = new CandidatesData
            {
                ApplicationUserId = user.Id,
                PositionId = positionId
            };

            _context.CandidatesData.Add(candidate);
            await _context.SaveChangesAsync();

            // Return success response
            return Ok();
        }

        // Removes a deployed candidate based on student ID
        [HttpPost]
        public async Task<IActionResult> DeleteCandidate([FromBody] string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return BadRequest("Invalid student ID.");

            // Find the user using student ID
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);
            if (user == null)
                return NotFound("User not found.");

            // Find candidate record using the user's ID
            var candidate = await _context.CandidatesData
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (candidate == null)
                return NotFound("Candidate not found or not deployed.");

            // Remove candidate from database
            _context.CandidatesData.Remove(candidate);
            await _context.SaveChangesAsync();

            // Return confirmation response
            return Ok("Candidate removed successfully.");
        }

        // Returns all organizations with their IDs and names
        public IActionResult GetOrganizations()
        {
            var organizations = _context.OrganizationsData
                .Select(o => new { o.Id, o.Name })
                .ToList();

            return Json(organizations);
        }

        // Returns positions within an organization that have not been assigned to a candidate
        public IActionResult GetAvailablePositions(int organizationId)
        {
            // Get all unassigned positions for the given organization
            var positions = _context.OrganizationPosition
                .Where(p => p.OrganizationId == organizationId)
                .Where(p => !_context.CandidatesData.Any(c => c.PositionId == p.Id))
                .Select(p => new { p.Id, p.Title })
                .ToList();

            return Json(positions);
        }
    }
}
