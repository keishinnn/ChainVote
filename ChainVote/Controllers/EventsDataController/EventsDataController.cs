using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.ViewModels;

namespace ChainVote.Controllers.EventsDataController
{
    [Authorize(Roles = "Admin")]
    public class EventsDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventsDataController> _logger;

        public EventsDataController(ApplicationDbContext context, ILogger<EventsDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Elections()
        {
            var allEvents = await _context.EventsData.ToListAsync();
            var allVoters = await _context.Voters.ToListAsync();

            var grouped = allEvents.Select(evt =>
            {
                var voters = allVoters.Where(v => v.EventId == evt.Id).ToList();
                return new ElectionSummary
                {
                    Event = evt,
                    TotalVoters = voters.Count,
                    TotalVoted = voters.Count(v => v.HasVoted)
                };
            }).ToList();

            var model = new ElectionOverviewViewModel
            {
                AwaitingElections = grouped.Where(e => e.Event.Status == "Awaiting").ToList(),
                InProgressElections = grouped.Where(e => e.Event.Status == "InProgress").ToList(),
                CompletedElections = grouped.Where(e => e.Event.Status == "Completed").ToList(),
                NewEvent = new EventsData
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1)
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddElection(ElectionOverviewViewModel model)
        {
            _logger.LogInformation("AddElection called at {Time}", DateTime.Now);

            if (model.NewEvent != null)
            {
                model.NewEvent.Status ??= "Awaiting";
                model.NewEvent.Email ??= User.Identity?.Name ?? "admin@example.com";
                model.NewEvent.Organizations ??= "DefaultOrganization";
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.NewEvent.StartDate >= model.NewEvent.EndDate)
                    {
                        TempData["ErrorMessage"] = "Start Date must be before End Date.";
                        return RedirectToAction("Elections");
                    }

                    _context.EventsData.Add(model.NewEvent);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Election created successfully!";
                    return RedirectToAction("Elections");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in AddElection");
                    TempData["ErrorMessage"] = "An error occurred while creating the election: " + ex.Message;
                    return RedirectToAction("Elections");
                }
            }

            _logger.LogWarning("ModelState is invalid");

            var allEvents = await _context.EventsData.ToListAsync();
            var allVoters = await _context.Voters.ToListAsync();

            var grouped = allEvents.Select(evt =>
            {
                var voters = allVoters.Where(v => v.EventId == evt.Id).ToList();
                return new ElectionSummary
                {
                    Event = evt,
                    TotalVoters = voters.Count,
                    TotalVoted = voters.Count(v => v.HasVoted)
                };
            }).ToList();

            model.AwaitingElections = grouped.Where(e => e.Event.Status == "Awaiting").ToList();
            model.InProgressElections = grouped.Where(e => e.Event.Status == "InProgress").ToList();
            model.CompletedElections = grouped.Where(e => e.Event.Status == "Completed").ToList();

            model.NewEvent ??= new EventsData
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Status = "Awaiting",
                Organizations = "DefaultOrganization"
            };

            TempData["ErrorMessage"] = "Failed to create election. Please check the form and try again.";
            return View("Elections", model);
        }

        public IActionResult EditElection(int id)
        {
            ViewBag.ElectionId = id;
            return View();
        }

        public IActionResult StopElection(int id)
        {
            ViewBag.ElectionId = id;
            // Logic to stop election (e.g., update status to "Stopped")
            return RedirectToAction("Elections");
        }
    }
}
