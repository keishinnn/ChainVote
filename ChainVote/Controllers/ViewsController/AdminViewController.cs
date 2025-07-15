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

        // Render the VoteRecords view with a list of vote records from the database
        public IActionResult VoteRecords()
        {
            // Fetch detailed vote records including related entities
            var voteRecords = FetchVoteRecords();
            return View(voteRecords);
        }

        // Render the Logout view
        public IActionResult Logout()
        {
            return View("Logout");
        }

        // Helper method to fetch vote records with related voter, candidate, position, organization, and event data
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

        // Display a list of users who have vote records, allowing admin to select users to delete votes from
        public IActionResult DeleteUserVotesView()
        {
            // Group votes by voter and select unique users with votes
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

        // Retrieve distinct election events that a specific user has voted in
        [HttpGet]
        public IActionResult GetUserElectionEvents(string userId)
        {
            // Query vote records to find all unique events the user has participated in
            var elections = _context.VoteRecords
                .Where(v => v.Voter.Id == userId)
                .Select(v => new
                {
                    Id = v.Candidate.Position.Organization.Event.Id,
                    Name = v.Candidate.Position.Organization.Event.EventName
                })
                .Distinct()
                .ToList()
                // Map events to SelectListItem for dropdown use in the view
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToList();

            // Return the events as JSON
            return Json(elections);
        }

        // Delete all votes cast by a user in a specific election event
        [HttpPost]
        public IActionResult DeleteUserVotes(string userId, int eventId)
        {
            // Find all vote records matching the user and event
            var votes = _context.VoteRecords
                .Include(v => v.Candidate)
                    .ThenInclude(c => c.Position)
                        .ThenInclude(p => p.Organization)
                .Where(v => v.Voter.Id == userId && v.Candidate.Position.Organization.EventId == eventId)
                .ToList();

            // Remove the retrieved vote records from the database
            _context.VoteRecords.RemoveRange(votes);
            _context.SaveChanges();

            // Provide feedback message for the successful operation
            TempData["Message"] = "User votes deleted successfully.";
            return RedirectToAction("DeleteUserVotesView");
        }
    }
}
