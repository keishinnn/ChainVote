using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChainVote.Controllers
{
    [Authorize(Roles = "Voter")] // Optional: use role-based access
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

        public IActionResult UserViewElections()
        {
            var allEvents = _context.EventsData.Include(e => e.VoteRecords).ToList();

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

            return View("~/Views/UserView/Index.cshtml", viewModel);
        }

        public IActionResult UserViewElectionStats(int eventId)
        {
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == eventId);
            if (eventData == null)
                return NotFound();

            var allowedSections = eventData.AllowedSections?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var allowedYears = eventData.AllowedYearLevels?.Split(',')
                                .Select(s => s.Trim()
                                    .Replace("1st Year", "1")
                                    .Replace("2nd Year", "2")
                                    .Replace("3rd Year", "3")
                                    .Replace("4th Year", "4"))
                                .ToList() ?? new();
            var allowedCourses = eventData.AllowedCourses?.Split(',').Select(s => s.Trim()).ToList() ?? new();

            var totalEligibleVoters = _context.Users.Count(u =>
                u.Section != null && u.YearLevel != null && u.Course != null &&
                allowedSections.Contains(u.Section.Trim()) &&
                allowedYears.Contains(u.YearLevel.Trim()) &&
                allowedCourses.Contains(u.Course.Trim())
            );

            var candidates = _context.CandidatesData
                .Include(c => c.Position)
                .Include(c => c.ApplicationUser)
                .Where(c => c.Position.Organization.EventId == eventId)
                .ToList();

            var votes = _context.VoteRecords
                .Where(v => v.EventId == eventId)
                .ToList();

            var viewModel = new ElectionStatsViewModel
            {
                EventId = eventId,
                EventTitle = eventData.EventName,
                EventEndDate = eventData.EndDate,
                EventStartDate = eventData.StartDate,
                TotalVoters = totalEligibleVoters,
                VotesCast = votes.Select(v => v.VoterId).Distinct().Count(),
                VoterTurnoutPercent = totalEligibleVoters == 0 ? 0 : (votes.Select(v => v.VoterId).Distinct().Count() * 100.0 / totalEligibleVoters),
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
            return View("~/Views/UserView/UserViewLiveStats.cshtml", viewModel);
        }

        public IActionResult DefaultViewElectionResults(int eventId)
        {
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == eventId);
            if (eventData == null)
                return NotFound();

            var allowedSections = eventData.AllowedSections?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var allowedYears = eventData.AllowedYearLevels?.Split(',')
                                .Select(s => s.Trim()
                                    .Replace("1st Year", "1")
                                    .Replace("2nd Year", "2")
                                    .Replace("3rd Year", "3")
                                    .Replace("4th Year", "4"))
                                .ToList() ?? new();
            var allowedCourses = eventData.AllowedCourses?.Split(',').Select(s => s.Trim()).ToList() ?? new();

            var totalEligibleVoters = _context.Users.Count(u =>
                u.Section != null && u.YearLevel != null && u.Course != null &&
                allowedSections.Contains(u.Section.Trim()) &&
                allowedYears.Contains(u.YearLevel.Trim()) &&
                allowedCourses.Contains(u.Course.Trim())
            );

            var candidates = _context.CandidatesData
                .Include(c => c.Position)
                .Include(c => c.ApplicationUser)
                .Where(c => c.Position.Organization.EventId == eventId)
                .ToList();

            var votes = _context.VoteRecords
                .Where(v => v.EventId == eventId)
                .ToList();

            var viewModel = new ElectionStatsViewModel
            {
                EventId = eventId,
                EventTitle = eventData.EventName,
                EventEndDate = eventData.EndDate,
                EventStartDate = eventData.StartDate,
                TotalVoters = totalEligibleVoters,
                VotesCast = votes.Select(v => v.VoterId).Distinct().Count(),
                VoterTurnoutPercent = totalEligibleVoters == 0 ? 0 : (votes.Select(v => v.VoterId).Distinct().Count() * 100.0 / totalEligibleVoters),
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
            return View("~/Views/UserView/UserViewElectionResults.cshtml", viewModel);
        }
    }
}
