using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChainVote.Controllers.ViewsController
{
    [Authorize(Roles = "Admin")]
    public class AdminViewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminViewController> _logger;

        public AdminViewController(ApplicationDbContext context, ILogger<AdminViewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. Dashboard
        public IActionResult Dashboard()
        {
            return RedirectToAction("Dashboard", "ElectionStats");
        }

        public IActionResult Elections()
        {
            return RedirectToAction("Elections", "EventsData");
        }

        public IActionResult EditVoter(int id)
        {
            ViewBag.VoterId = id;
            return View();
        }

        public IActionResult Organizations()
        {
            return RedirectToAction("Organizations", "OrganizationsData");
        }

        public IActionResult Candidates()
        {
            return View();
        }

        public IActionResult Accounts()
        {
            return View();
        }

        public IActionResult Voters()
        {
            return View();
        }
        public IActionResult VoteRecords()
        {
            var voteRecords = FetchVoteRecords();
            return View(voteRecords);
        }

        // 7. Logout
        public IActionResult Logout()
        {
            // Logic to sign out the user (optional)
            return View("Logout");
        }

        private List<VoteRecordViewModel> FetchVoteRecords()
        {
            var records = _context.VoteRecords
                .Include(v => v.Voter)
                .Include(v => v.Candidate)
                    .ThenInclude(c => c.ApplicationUser)
                .Include(v => v.Candidate.Position)
                    .ThenInclude(p => p.Organization)
                        .ThenInclude(o => o.Event)
                .Select(v => new VoteRecordViewModel
                {
                    VoterEmail = v.Voter.Email,
                    CandidateName = v.Candidate.ApplicationUser.FirstName + " " + v.Candidate.ApplicationUser.LastName,
                    PositionName = v.Candidate.Position.Title,
                    OrganizationName = v.Candidate.Position.Organization.Name,
                    EventName = v.Candidate.Position.Organization.Event.EventName,
                    VotedAt = v.VotedAt
                })
                .ToList();

            return records;
        }

        public IActionResult DeleteUserVotesView()
        {
            var usersWithVotes = _context.VoteRecords
                .Include(v => v.Voter)
                .GroupBy(v => v.Voter.Id)
                .Select(g => new UserWithVotesViewModel
                {
                    UserId = g.Key,
                    Email = g.First().Voter.Email,
                    FullName = g.First().Voter.FirstName + " " + g.First().Voter.LastName
                })
                .ToList();

            return View(usersWithVotes);
        }

        [HttpGet]
        public IActionResult GetUserElectionEvents(string userId)
        {
            var elections = _context.VoteRecords
                .Where(v => v.Voter.Id == userId)
                .Select(v => new
                {
                    Id = v.Candidate.Position.Organization.Event.Id,
                    Name = v.Candidate.Position.Organization.Event.EventName
                })
                .Distinct()
                .ToList()
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToList();

            return Json(elections);
        }

        [HttpPost]
        public IActionResult DeleteUserVotes(string userId, int eventId)
        {
            var votes = _context.VoteRecords
                .Include(v => v.Candidate)
                    .ThenInclude(c => c.Position)
                        .ThenInclude(p => p.Organization)
                .Where(v => v.Voter.Id == userId && v.Candidate.Position.Organization.EventId == eventId)
                .ToList();

            _context.VoteRecords.RemoveRange(votes);
            _context.SaveChanges();

            TempData["Message"] = "User votes deleted successfully.";
            return RedirectToAction("DeleteUserVotesView");
        }
    }
}
