using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChainVote.Controllers
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
            // Fetch all election events and all users from the database asynchronously
            var allEvents = await _context.EventsData.ToListAsync();
            var allUsers = await _context.Users.ToListAsync();

            // Group events with voter statistics: total allowed voters and how many voted
            var grouped = allEvents.Select(evt =>
            {
                // Parse allowed year levels, sections, and courses from comma-separated strings
                var allowedYearLevels = evt.AllowedYearLevels?.Split(',') ?? new string[] { };
                var allowedSections = evt.AllowedSections?.Split(',') ?? new string[] { };
                var allowedCourses = evt.AllowedCourses?.Split(',') ?? new string[] { };

                // Filter users who match the allowed criteria for this event
                var voters = allUsers.Where(u =>
                    allowedYearLevels.Contains(u.YearLevel) &&
                    allowedSections.Contains(u.Section) &&
                    allowedCourses.Contains(u.Course)
                ).ToList();

                // Return a summary object containing event and voter counts
                return new ElectionSummary
                {
                    Event = evt,
                    TotalVoters = voters.Count,
                    TotalVoted = voters.Count(v => v.HasVoted)
                };
            }).ToList();

            // Prepare the view model grouping elections by their status and initializing a new event with default dates
            var model = new ElectionOverviewViewModel
            {
                AwaitingElections = grouped
                    .Where(e => e.Event.Status == ElectionStatus.Awaiting)
                    .ToList(),

                InProgressElections = grouped
                    .Where(e => e.Event.Status == ElectionStatus.InProgress)
                    .ToList(),

                CompletedElections = grouped
                    .Where(e => e.Event.Status == ElectionStatus.Completed)
                    .ToList(),

                NewEvent = new EventsData
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1)
                },
            };

            // Render the Elections view in the Admin area with the prepared model
            return View("~/Views/AdminView/Elections.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddElection(ElectionOverviewViewModel model, List<string> Courses, List<string> YearLevels, List<string> Sections)
        {
            _logger.LogInformation("AddElection called at {Time}", DateTime.Now);

            if (model.NewEvent != null)
            {
                // Set default status and email for new election event
                model.NewEvent.Status = ElectionStatus.Awaiting;
                model.NewEvent.Email ??= User.Identity?.Name ?? "admin@example.com";
                model.NewEvent.Organizations ??= null;

                // Join allowed voters selections into comma-separated strings or empty if null
                model.NewEvent.AllowedCourses = Courses != null ? string.Join(",", Courses) : "";
                model.NewEvent.AllowedYearLevels = YearLevels != null ? string.Join(",", YearLevels) : "";
                model.NewEvent.AllowedSections = Sections != null ? string.Join(",", Sections) : "";
            }

            // Validate the model state, return view with errors if invalid
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));

                return View("~/Views/AdminView/Elections.cshtml", model);
            }

            try
            {
                _logger.LogInformation("Start Date: {StartDate}, End Date: {EndDate}",
                    model.NewEvent.StartDate, model.NewEvent.EndDate);

                // Check that start date is before end date; redirect with error if not
                if (model.NewEvent.StartDate >= model.NewEvent.EndDate)
                {
                    TempData["ErrorMessage"] = "Start Date must be before End Date.";
                    return RedirectToAction("Elections");
                }

                // Add new event to the database context and save changes
                _context.EventsData.Add(model.NewEvent);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Election created successfully!";
                return RedirectToAction("Elections");
            }
            catch (Exception ex)
            {
                // Log any exceptions and show error message to user
                _logger.LogError(ex, "Exception in AddElection");
                TempData["ErrorMessage"] = "An error occurred while creating the election: " + ex.Message;
                return RedirectToAction("Elections");
            }
        }

        public IActionResult EditElection(int id)
        {
            // Pass the election ID to the view for editing (view to be implemented)
            ViewBag.ElectionId = id;
            return View();
        }

        public IActionResult StopElection(int id)
        {
            // Placeholder for stopping election logic, then redirect back to elections page
            ViewBag.ElectionId = id;
            return RedirectToAction("Elections");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteElection(int id)
        {
            // Find the election event by ID
            var eventToDelete = await _context.EventsData.FindAsync(id);

            if (eventToDelete == null)
            {
                // Show error if election does not exist
                TempData["ErrorMessage"] = "Election not found.";
                return RedirectToAction("Elections");
            }

            try
            {
                // Remove election from database and save changes
                _context.EventsData.Remove(eventToDelete);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Election deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log any errors during deletion and notify user
                _logger.LogError(ex, "Error deleting election");
                TempData["ErrorMessage"] = "Error deleting election: " + ex.Message;
            }

            // Redirect back to the elections overview page
            return RedirectToAction("Elections");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllowedVoters(int ElectionId, List<string> YearLevels, List<string> Sections, List<string> Courses)
        {
            // Retrieve the specific election event by ID
            var eventData = await _context.EventsData.FindAsync(ElectionId);
            if (eventData == null)
            {
                TempData["ErrorMessage"] = "Election not found.";
                return RedirectToAction("Elections", "AdminView");
            }

            // Validate that all lists have at least one selected value
            if ((YearLevels == null || YearLevels.Count == 0) ||
                (Sections == null || Sections.Count == 0) ||
                (Courses == null || Courses.Count == 0))
            {
                TempData["ErrorMessage"] = "Please select at least one Year Level, Section, and Course.";
                return RedirectToAction("Elections", "AdminView");
            }

            // Handle 'All' selection or join the selections into comma-separated strings
            eventData.AllowedYearLevels = YearLevels.Contains("All") ? "All" : string.Join(",", YearLevels);
            eventData.AllowedSections = Sections.Contains("All") ? "All" : string.Join(",", Sections);
            eventData.AllowedCourses = Courses.Contains("All") ? "All" : string.Join(",", Courses);

            try
            {
                // Update event data in database and save changes
                _context.Update(eventData);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Allowed voters updated successfully.";
            }
            catch (Exception ex)
            {
                // Handle and log any errors during update
                TempData["ErrorMessage"] = "Failed to update allowed voters.";
            }

            // Redirect back to elections page after update
            return RedirectToAction("Elections", "AdminView");
        }

        public IActionResult GetAllowedVotersData(int electionId)
        {
            // Define predefined lists of courses, year levels, and sections for selection UI
            var predefinedCourses = new List<string> { "BSCS", "BSIT", "BSIS", "BSDS", "BSEMC", "BSCpE" };
            var predefinedYearLevels = new List<string> { "1st Year", "2nd Year", "3rd Year", "4th Year" };
            var predefinedSections = Enumerable.Range('A', 'P' - 'A' + 1).Select(c => ((char)c).ToString()).ToList();

            // Retrieve election event by ID to get currently allowed voters data
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == electionId);

            // Parse the allowed voters from stored comma-separated strings into lists
            var selectedCourses = eventData?.AllowedCourses?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList() ?? new List<string>();
            var selectedYearLevels = eventData?.AllowedYearLevels?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList() ?? new List<string>();
            var selectedSections = eventData?.AllowedSections?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList() ?? new List<string>();

            // Return the predefined lists and current selections as JSON for client-side use
            return Json(new
            {
                predefinedCourses,
                predefinedYearLevels,
                predefinedSections,
                selectedCourses,
                selectedYearLevels,
                selectedSections
            });
        }
    }
}
