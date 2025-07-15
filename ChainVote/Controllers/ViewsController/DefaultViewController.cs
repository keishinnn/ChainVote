using System.Diagnostics;
using ChainVote.Data;
using ChainVote.Models;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainVote.Controllers.ViewsController
{
    public class DefaultViewController : Controller
    {
        private readonly ILogger<DefaultViewController> _logger;
        private readonly ApplicationDbContext _context;

        public DefaultViewController(ApplicationDbContext context, ILogger<DefaultViewController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Return error view with the current request ID for debugging
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DefaultViewElections()
        {
            // Fetch all election events including their vote records from the database
            var allEvents = _context.EventsData.Include(e => e.VoteRecords).ToList();

            // Prepare the ViewModel separating elections by status with summary info
            var viewModel = new ElectionOverviewViewModel
            {
                // Elections that are currently in progress with total voters and votes counted
                InProgressElections = allEvents
                    .Where(e => e.Status == ElectionStatus.InProgress)
                    .Select(e => new ElectionSummary
                    {
                        Event = e,
                        TotalVoters = e.VoteRecords.Select(v => v.VoterId).Distinct().Count(),
                        TotalVoted = e.VoteRecords.Count()
                    }).ToList(),

                // Elections that are completed with total voters and votes counted
                CompletedElections = allEvents
                    .Where(e => e.Status == ElectionStatus.Completed)
                    .Select(e => new ElectionSummary
                    {
                        Event = e,
                        TotalVoters = e.VoteRecords.Select(v => v.VoterId).Distinct().Count(),
                        TotalVoted = e.VoteRecords.Count()
                    }).ToList()
            };

            // Return the elections overview view with the prepared data
            return View("~/Views/DefaultView/DefaultViewElections.cshtml", viewModel);
        }

        public IActionResult DefaultViewElectionStats(int eventId)
        {
            // Retrieve the election event by ID
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == eventId);

            // If the event does not exist, return 404 Not Found
            if (eventData == null)
                return NotFound();

            // Parse allowed sections, years, and courses from the event's comma-separated strings
            var allowedSections = eventData.AllowedSections?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var allowedYears = eventData.AllowedYearLevels?.Split(',')
                                .Select(s => s.Trim()
                                    .Replace("1st Year", "1")
                                    .Replace("2nd Year", "2")
                                    .Replace("3rd Year", "3")
                                    .Replace("4th Year", "4"))
                                .ToList() ?? new();
            var allowedCourses = eventData.AllowedCourses?.Split(',').Select(s => s.Trim()).ToList() ?? new();

            // Count total eligible voters who match the allowed criteria for this election
            var totalEligibleVoters = _context.Users.Count(u =>
                u.Section != null && u.YearLevel != null && u.Course != null &&
                allowedSections.Contains(u.Section.Trim()) &&
                allowedYears.Contains(u.YearLevel.Trim()) &&
                allowedCourses.Contains(u.Course.Trim())
            );

            // Fetch candidates related to this election event including position and user details
            var candidates = _context.CandidatesData
                .Include(c => c.Position)
                .Include(c => c.ApplicationUser)
                .Where(c => c.Position.Organization.EventId == eventId)
                .ToList();

            // Fetch all votes cast in this election event
            var votes = _context.VoteRecords
                .Where(v => v.EventId == eventId)
                .ToList();

            // Prepare the statistics view model to show election details and candidate stats
            var viewModel = new ElectionStatsViewModel
            {
                EventId = eventId,
                EventTitle = eventData.EventName,
                EventEndDate = eventData.EndDate,
                EventStartDate = eventData.StartDate,
                TotalVoters = totalEligibleVoters,
                VotesCast = votes.Select(v => v.VoterId).Distinct().Count(),
                VoterTurnoutPercent = totalEligibleVoters == 0 ? 0 : (votes.Select(v => v.VoterId).Distinct().Count() * 100.0 / totalEligibleVoters),

                // Group candidates by position and summarize their vote counts
                Positions = candidates
                    .GroupBy(c => c.Position.Title)
                    .Select(g => new PositionStatsViewModel
                    {
                        PositionTitle = g.Key,
                        Candidates = g.Select(c => new CandidateStatsViewModel
                        {
                            CandidateName = $"{c.ApplicationUser.FirstName} {c.ApplicationUser.LastName}",
                            VoteCount = votes.Count(v => v.CandidateId == c.Id)
                        }).ToList()
                    }).ToList()
            };

            // Return the live stats view with the compiled election statistics
            return View("~/Views/DefaultView/DefaultViewLiveStats.cshtml", viewModel);
        }

        public IActionResult DefaultViewElectionResults(int eventId)
        {
            // Retrieve the election event by ID
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == eventId);

            // Return 404 if event not found
            if (eventData == null)
                return NotFound();

            // Parse allowed sections, years, and courses from event data
            var allowedSections = eventData.AllowedSections?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var allowedYears = eventData.AllowedYearLevels?.Split(',')
                                .Select(s => s.Trim()
                                    .Replace("1st Year", "1")
                                    .Replace("2nd Year", "2")
                                    .Replace("3rd Year", "3")
                                    .Replace("4th Year", "4"))
                                .ToList() ?? new();
            var allowedCourses = eventData.AllowedCourses?.Split(',').Select(s => s.Trim()).ToList() ?? new();

            // Count total eligible voters matching the event's criteria
            var totalEligibleVoters = _context.Users.Count(u =>
                u.Section != null && u.YearLevel != null && u.Course != null &&
                allowedSections.Contains(u.Section.Trim()) &&
                allowedYears.Contains(u.YearLevel.Trim()) &&
                allowedCourses.Contains(u.Course.Trim())
            );

            // Fetch candidates for the event including position and user info
            var candidates = _context.CandidatesData
                .Include(c => c.Position)
                .Include(c => c.ApplicationUser)
                .Where(c => c.Position.Organization.EventId == eventId)
                .ToList();

            // Fetch votes cast for the event
            var votes = _context.VoteRecords
                .Where(v => v.EventId == eventId)
                .ToList();

            // Build the election results view model with detailed vote counts per candidate
            var viewModel = new ElectionStatsViewModel
            {
                EventId = eventId,
                EventTitle = eventData.EventName,
                EventEndDate = eventData.EndDate,
                EventStartDate = eventData.StartDate,
                TotalVoters = totalEligibleVoters,
                VotesCast = votes.Select(v => v.VoterId).Distinct().Count(),
                VoterTurnoutPercent = totalEligibleVoters == 0 ? 0 : (votes.Select(v => v.VoterId).Distinct().Count() * 100.0 / totalEligibleVoters),

                // Group candidates by position with their vote tallies
                Positions = candidates
                    .GroupBy(c => c.Position.Title)
                    .Select(g => new PositionStatsViewModel
                    {
                        PositionTitle = g.Key,
                        Candidates = g.Select(c => new CandidateStatsViewModel
                        {
                            CandidateName = $"{c.ApplicationUser.FirstName} {c.ApplicationUser.LastName}",
                            VoteCount = votes.Count(v => v.CandidateId == c.Id)
                        }).ToList()
                    }).ToList()
            };

            // Return the election results view populated with the model
            return View("~/Views/DefaultView/DefaultViewElectionResults.cshtml", viewModel);
        }
    }
}
