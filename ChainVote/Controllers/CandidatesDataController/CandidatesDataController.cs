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

        // Method to get eligible (non-candidate, non-admin) users
        [HttpGet]
        public IActionResult GetReadyForDeployment()
        {
            // Get all user IDs already in CandidatesData
            var candidateUserIds = _context.CandidatesData
                .Select(c => c.ApplicationUserId)
                .ToList();

            // Get all user IDs with the "Admin" role
            var adminRoleId = _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefault();

            var adminUserIds = _context.UserRoles
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .ToList();

            // Filter users: exclude both admins and those already in CandidatesData
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

            return Json(new { data = eligibleCandidates });
        }


        [HttpPost]
        public async Task<IActionResult> DeployCandidate(string studentId, int positionId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);

            if (user == null)
                return NotFound();

            // Check if already a candidate
            var exists = await _context.CandidatesData.AnyAsync(c => c.ApplicationUserId == user.Id);
            if (exists)
                return BadRequest("User is already a candidate.");

            // Check if position is already taken
            var positionTaken = await _context.CandidatesData.AnyAsync(c => c.PositionId == positionId);
            if (positionTaken)
                return BadRequest("This position is already assigned to another candidate.");

            // Create candidate
            var candidate = new CandidatesData
            {
                ApplicationUserId = user.Id,
                PositionId = positionId
            };

            _context.CandidatesData.Add(candidate);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCandidate([FromBody] string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return BadRequest("Invalid student ID.");

            // Find the user by student ID
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentId);
            if (user == null)
                return NotFound("User not found.");

            // Find the deployed candidate associated with that user
            var candidate = await _context.CandidatesData
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (candidate == null)
                return NotFound("Candidate not found or not deployed.");

            // Remove the candidate
            _context.CandidatesData.Remove(candidate);
            await _context.SaveChangesAsync();

            return Ok("Candidate removed successfully.");
        }

        // Controller Action for fetching organizations
        public IActionResult GetOrganizations()
        {
            var organizations = _context.OrganizationsData
                .Select(o => new { o.Id, o.Name })
                .ToList();

            return Json(organizations);
        }

        public IActionResult GetAvailablePositions(int organizationId)
        {
            // Get all positions for the organization
            var positions = _context.OrganizationPosition
                .Where(p => p.OrganizationId == organizationId)
                .Where(p => !_context.CandidatesData.Any(c => c.PositionId == p.Id)) // Position not assigned
                .Select(p => new { p.Id, p.Title })
                .ToList();

            return Json(positions);
        }

    }
}
