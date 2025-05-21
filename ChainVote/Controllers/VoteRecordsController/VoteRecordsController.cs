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
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Find(userId);

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

            return Json(availableElections);
        }

        [HttpGet]
        public IActionResult GetPositionsAndCandidates(int eventId)
        {
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

            return Json(positions);
        }


        [HttpPost]
        public IActionResult SubmitVote([FromBody] VoteSubmissionDto submission)
        {
            var userId = _userManager.GetUserId(User);

            // Check if the user has already voted
            var alreadyVoted = _context.VoteRecords.Any(v => v.VoterId == userId && v.EventId == submission.EventId);
            if (alreadyVoted)
                return BadRequest("You have already voted in this event.");

            foreach (var vote in submission.Votes)
            {
                // Get the candidate being voted for (include full path to Position and Organization and Event)
                var candidate = _context.CandidatesData
                    .Include(c => c.Position)
                        .ThenInclude(p => p.Organization)
                            .ThenInclude(o => o.Event)
                    .FirstOrDefault(c => c.Id == vote.CandidateId);

                if (candidate == null)
                    return BadRequest($"Candidate with ID {vote.CandidateId} not found.");

                // Check if the current voter is also a candidate in the same position
                var isVoterAlsoCandidateInSamePosition = _context.CandidatesData
                    .Any(c =>
                        c.ApplicationUserId == userId &&
                        c.PositionId == candidate.PositionId && // Same position
                        c.Position.Organization.Event.Id == submission.EventId); // Same event

                if (isVoterAlsoCandidateInSamePosition)
                    return BadRequest($"You cannot vote for the position '{candidate.Position.Title}' where you are a candidate.");

                // If passed, save the vote
                _context.VoteRecords.Add(new VoteRecord
                {
                    VoterId = userId,
                    CandidateId = vote.CandidateId,
                    EventId = submission.EventId,
                    VotedAt = DateTime.UtcNow
                });
            }

            var user = _context.Users.Find(userId);
            user.HasVoted = true;

            _context.SaveChanges();
            return Ok();
        }

        public IActionResult VoteForm(int eventId)
        {
            _logger.LogInformation("VoteForm called with eventId: {EventId}", eventId);
            var userId = _userManager.GetUserId(User);
            var alreadyVoted = _context.VoteRecords.Any(v => v.VoterId == userId && v.EventId == eventId);

            ViewBag.AlreadyVoted = alreadyVoted;
            ViewBag.EventId = eventId;

            return View("~/Views/UserView/VoteForm.cshtml");
        }

        [HttpGet]
        public IActionResult HasVoted(int eventId)
        {
            var userId = _userManager.GetUserId(User);
            var alreadyVoted = _context.VoteRecords.Any(v => v.VoterId == userId && v.EventId == eventId);
            return Json(new { alreadyVoted });
        }

    }
}
