using System.Linq;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.Dto;
using ChainVote.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainVote.Controllers
{
    [Authorize(Roles = "Voter")]
    public class VoteRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VoteRecordsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public VoteRecordsController(ApplicationDbContext context, ILogger<VoteRecordsController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAvailableElections()
        {
            // Get the currently logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch the user's profile details from the database
            var user = _context.Users.Find(userId);

            // Retrieve all active election events that match the user's course, year level, and section
            var availableElections = _context.EventsData
                .Where(e =>
                    (string.IsNullOrEmpty(e.AllowedCourses) || e.AllowedCourses.Contains(user.Course)) &&
                    (string.IsNullOrEmpty(e.AllowedYearLevels) || e.AllowedYearLevels.Contains(user.YearLevel)) &&
                    (string.IsNullOrEmpty(e.AllowedSections) || e.AllowedSections.Contains(user.Section)) &&
                    e.Status == ElectionStatus.InProgress
                )
                .Select(e => new
                {
                    e.Id,
                    e.EventName
                })
                .ToList();

            // Return the list of elections as JSON
            return Json(availableElections);
        }

        [HttpGet]
        public IActionResult GetPositionsAndCandidates(int eventId)
        {
            // Retrieve all positions and their candidates under the specified election event
            var positions = _context.OrganizationPosition
                .Where(p => p.Organization.EventId == eventId)
                .Select(p => new PositionDto
                {
                    PositionId = p.Id,
                    PositionTitle = p.Title,
                    Candidates = p.Candidates.Select(c => new CandidateDto
                    {
                        CandidateId = c.Id,
                        FullName = c.ApplicationUser.FirstName + " " + c.ApplicationUser.LastName,
                        OrganizationName = p.Organization.Name
                    }).ToList()
                }).ToList();

            // Return the positions and candidates as JSON
            return Json(positions);
        }

        [HttpPost]
        public IActionResult SubmitVote([FromBody] VoteSubmissionDto submission)
        {
            // Get the logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Prevent users from voting more than once in the same event
            var alreadyVoted = _context.VoteRecords.Any(v => v.VoterId == userId && v.EventId == submission.EventId);
            if (alreadyVoted)
                return BadRequest("You have already voted in this event.");

            // Process each vote in the submission
            foreach (var vote in submission.Votes)
            {
                // Look up the candidate by ID, including the full relationship path to the event
                var candidate = _context.CandidatesData
                    .Include(c => c.Position)
                        .ThenInclude(p => p.Organization)
                            .ThenInclude(o => o.Event)
                    .FirstOrDefault(c => c.Id == vote.CandidateId);

                // Ensure the candidate exists
                if (candidate == null)
                    return BadRequest($"Candidate with ID {vote.CandidateId} not found.");

                // Prevent users from voting in positions where they are also candidates
                var isVoterAlsoCandidateInSamePosition = _context.CandidatesData
                    .Any(c =>
                        c.ApplicationUserId == userId &&
                        c.PositionId == candidate.PositionId &&
                        c.Position.Organization.Event.Id == submission.EventId);

                if (isVoterAlsoCandidateInSamePosition)
                    return BadRequest($"You cannot vote for the position '{candidate.Position.Title}' where you are a candidate.");

                // Save the vote to the database
                _context.VoteRecords.Add(new VoteRecord
                {
                    VoterId = userId,
                    CandidateId = vote.CandidateId,
                    EventId = submission.EventId,
                    VotedAt = DateTime.UtcNow
                });
            }

            // Update the user's profile to indicate they have voted
            var user = _context.Users.Find(userId);
            user.HasVoted = true;

            // Persist all changes
            _context.SaveChanges();
            return Ok();
        }

        public IActionResult VoteForm(int eventId)
        {
            // Log that the VoteForm is being accessed with a specific event ID
            _logger.LogInformation("VoteForm called with eventId: {EventId}", eventId);

            // Check if the user has already voted in this event
            var userId = _userManager.GetUserId(User);
            var alreadyVoted = _context.VoteRecords.Any(v => v.VoterId == userId && v.EventId == eventId);

            // Pass information to the view using ViewBag
            ViewBag.AlreadyVoted = alreadyVoted;
            ViewBag.EventId = eventId;

            // Return the vote form view
            return View("~/Views/UserView/VoteForm.cshtml");
        }

        [HttpGet]
        public IActionResult HasVoted(int eventId)
        {
            // Check if the current user has already voted in the given event
            var userId = _userManager.GetUserId(User);
            var alreadyVoted = _context.VoteRecords.Any(v => v.VoterId == userId && v.EventId == eventId);

            // Return voting status as JSON
            return Json(new { alreadyVoted });
        }
    }
}
