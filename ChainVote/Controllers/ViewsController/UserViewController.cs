using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChainVote.Controllers
{
    [Authorize(Roles = "Voter")]
    public class UserViewController : Controller
    {
        private readonly ILogger<UserViewController> _logger;
        private readonly ApplicationDbContext _context;

        public UserViewController(ApplicationDbContext context, ILogger<UserViewController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Elections()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Vote()
        {
            return View();
        }

        public IActionResult EditEmail()
        {
            return View();
        }

        public IActionResult EditPassword()
        {
            return View();
        }

        public IActionResult ViewProfile()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        // Loads the list of elections available to the voter and separates them by status
        public IActionResult UserViewElections()
        {
            // Fetch all election events including their vote records
            var allEvents = _context.EventsData.Include(e => e.VoteRecords).ToList();

            // Prepare the view model with in-progress and completed elections
            var viewModel = new ElectionOverviewViewModel
            {
                InProgressElections = allEvents
                    .Where(e => e.Status == ElectionStatus.InProgress)
                    .Select(e => new ElectionSummary
                    {
                        Event = e,
                        TotalVoters = e.VoteRecords.Select(v => v.VoterId).Distinct().Count(),
                        TotalVoted = e.VoteRecords.Count()
                    }).ToList(),

                CompletedElections = allEvents
                    .Where(e => e.Status == ElectionStatus.Completed)
                    .Select(e => new ElectionSummary
                    {
                        Event = e,
                        TotalVoters = e.VoteRecords.Select(v => v.VoterId).Distinct().Count(),
                        TotalVoted = e.VoteRecords.Count()
                    }).ToList()
            };

            // Return the user view election index page with the compiled data
            return View("~/Views/UserView/Index.cshtml", viewModel);
        }

        // Displays live statistics for a specific election event
        public IActionResult UserViewElectionStats(int eventId)
        {
            // Fetch the event by ID
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == eventId);
            if (eventData == null)
                return NotFound();

            // Parse allowed sections, year levels, and courses
            var allowedSections = eventData.AllowedSections?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var allowedYears = eventData.AllowedYearLevels?.Split(',')
                                .Select(s => s.Trim()
                                    .Replace("1st Year", "1")
                                    .Replace("2nd Year", "2")
                                    .Replace("3rd Year", "3")
                                    .Replace("4th Year", "4"))
                                .ToList() ?? new();
            var allowedCourses = eventData.AllowedCourses?.Split(',').Select(s => s.Trim()).ToList() ?? new();

            // Count eligible voters based on event restrictions
            var totalEligibleVoters = _context.Users.Count(u =>
                u.Section != null && u.YearLevel != null && u.Course != null &&
                allowedSections.Contains(u.Section.Trim()) &&
                allowedYears.Contains(u.YearLevel.Trim()) &&
                allowedCourses.Contains(u.Course.Trim())
            );

            // Retrieve all candidates for this event
            var candidates = _context.CandidatesData
                .Include(c => c.Position)
                .Include(c => c.ApplicationUser)
                .Where(c => c.Position.Organization.EventId == eventId)
                .ToList();

            // Get all vote records for the event
            var votes = _context.VoteRecords
                .Where(v => v.EventId == eventId)
                .ToList();

            // Construct the stats view model
            var viewModel = new ElectionStatsViewModel
            {
                EventId = eventId,
                EventTitle = eventData.EventName,
                EventEndDate = eventData.EndDate,
                EventStartDate = eventData.StartDate,
                TotalVoters = totalEligibleVoters,
                VotesCast = votes.Select(v => v.VoterId).Distinct().Count(),
                VoterTurnoutPercent = totalEligibleVoters == 0 ? 0 :
                    (votes.Select(v => v.VoterId).Distinct().Count() * 100.0 / totalEligibleVoters),
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

            // Return the live stats view with the computed statistics
            return View("~/Views/UserView/UserViewLiveStats.cshtml", viewModel);
        }

        // Displays the final results for a completed election event
        public IActionResult UserViewElectionResults(int eventId)
        {
            // Fetch the event by ID
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == eventId);
            if (eventData == null)
                return NotFound();

            // Parse allowed sections, year levels, and courses
            var allowedSections = eventData.AllowedSections?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var allowedYears = eventData.AllowedYearLevels?.Split(',')
                                .Select(s => s.Trim()
                                    .Replace("1st Year", "1")
                                    .Replace("2nd Year", "2")
                                    .Replace("3rd Year", "3")
                                    .Replace("4th Year", "4"))
                                .ToList() ?? new();
            var allowedCourses = eventData.AllowedCourses?.Split(',').Select(s => s.Trim()).ToList() ?? new();

            // Count eligible voters based on the event filters
            var totalEligibleVoters = _context.Users.Count(u =>
                u.Section != null && u.YearLevel != null && u.Course != null &&
                allowedSections.Contains(u.Section.Trim()) &&
                allowedYears.Contains(u.YearLevel.Trim()) &&
                allowedCourses.Contains(u.Course.Trim())
            );

            // Fetch all candidates and related user data
            var candidates = _context.CandidatesData
                .Include(c => c.Position)
                .Include(c => c.ApplicationUser)
                .Where(c => c.Position.Organization.EventId == eventId)
                .ToList();

            // Get all votes for the current event
            var votes = _context.VoteRecords
                .Where(v => v.EventId == eventId)
                .ToList();

            // Prepare the view model for the result page
            var viewModel = new ElectionStatsViewModel
            {
                EventId = eventId,
                EventTitle = eventData.EventName,
                EventEndDate = eventData.EndDate,
                EventStartDate = eventData.StartDate,
                TotalVoters = totalEligibleVoters,
                VotesCast = votes.Select(v => v.VoterId).Distinct().Count(),
                VoterTurnoutPercent = totalEligibleVoters == 0 ? 0 :
                    (votes.Select(v => v.VoterId).Distinct().Count() * 100.0 / totalEligibleVoters),
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

            // Return the election results view
            return View("~/Views/UserView/UserViewElectionResults.cshtml", viewModel);
        }
    }
}
