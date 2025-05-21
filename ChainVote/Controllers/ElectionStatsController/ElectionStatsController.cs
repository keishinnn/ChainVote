using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.Dto;
using ChainVote.Models.Identity;
using ChainVote.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainVote.Controllers.ElectionStatsController
{
    [Authorize(Roles = "Admin")]
    public class ElectionStatsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<ElectionStatsController> _logger;

        public ElectionStatsController(ApplicationDbContext context, ILogger<ElectionStatsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Dashboard()
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

            return View("~/Views/AdminView/Dashboard.cshtml", viewModel);
        }

        public IActionResult ViewElectionStats(int eventId)
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
            _logger.LogInformation("VoteForm called with total voters: {TotalVoters}", totalEligibleVoters);
            return View("~/Views/AdminView/LiveStats.cshtml", viewModel);
        }

        public IActionResult ElectionResults(int eventId)
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
            _logger.LogInformation("VoteForm called with total voters: {TotalVoters}", totalEligibleVoters);
            return View("~/Views/AdminView/ElectionResults.cshtml", viewModel);
        }

    }
}
