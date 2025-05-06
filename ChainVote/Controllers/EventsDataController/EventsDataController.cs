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
            var allEvents = await _context.EventsData.ToListAsync();
            var allUsers = await _context.Users.ToListAsync();

            var grouped = allEvents.Select(evt =>
            {
                // Parse the allowed values from the event
                var allowedYearLevels = evt.AllowedYearLevels?.Split(',') ?? new string[] { };
                var allowedSections = evt.AllowedSections?.Split(',') ?? new string[] { };
                var allowedCourses = evt.AllowedCourses?.Split(',') ?? new string[] { };

                // Filter users by matching the allowed criteria
                var voters = allUsers.Where(u =>
                    allowedYearLevels.Contains(u.YearLevel) &&
                    allowedSections.Contains(u.Section) &&
                    allowedCourses.Contains(u.Course)
                ).ToList();

                return new ElectionSummary
                {
                    Event = evt,
                    TotalVoters = voters.Count,
                    TotalVoted = voters.Count(v => v.HasVoted)
                };
            }).ToList();

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

                ElectionTypes = Enum.GetValues(typeof(ElectionType))
                    .Cast<ElectionType>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e == ElectionType.CampusGovernment ? "CSG/USG" : "Class Officer"
                    })
                    .ToList()
            };

            return View("~/Views/AdminView/Elections.cshtml", model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddElection(ElectionOverviewViewModel model, List<string> Courses, List<string> YearLevels, List<string> Sections)
        {
            _logger.LogInformation("AddElection called at {Time}", DateTime.Now);
            _logger.LogInformation("SelectedElectionType received: {Type}", model.SelectedElectionType);

            if (model.NewEvent != null)
            {
                model.NewEvent.Status = ElectionStatus.Awaiting;
                model.NewEvent.Email ??= User.Identity?.Name ?? "admin@example.com";
                model.NewEvent.Organizations ??= null;

                model.NewEvent.AllowedCourses = Courses != null ? string.Join(",", Courses) : "";
                model.NewEvent.AllowedYearLevels = YearLevels != null ? string.Join(",", YearLevels) : "";
                model.NewEvent.AllowedSections = Sections != null ? string.Join(",", Sections) : "";
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid: {Errors}", string.Join(", ",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));

                model.ElectionTypes = Enum.GetValues(typeof(ElectionType))
                    .Cast<ElectionType>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e == ElectionType.CampusGovernment ? "CSG/USG" : "Class Officer"
                    })
                    .ToList();

                return View("~/Views/AdminView/Elections.cshtml", model);
            }

            try
            {
                _logger.LogInformation("Start Date: {StartDate}, End Date: {EndDate}",
                    model.NewEvent.StartDate, model.NewEvent.EndDate);

                if (model.NewEvent.StartDate >= model.NewEvent.EndDate)
                {
                    TempData["ErrorMessage"] = "Start Date must be before End Date.";
                    return RedirectToAction("Elections");
                }

                if (Enum.TryParse<ElectionType>(model.SelectedElectionType, out var parsedElectionType)
                    && Enum.IsDefined(typeof(ElectionType), parsedElectionType))
                {
                    model.NewEvent.ElectionType = parsedElectionType;
                }
                else
                {
                    TempData["ErrorMessage"] = $"Invalid election type selected: {model.SelectedElectionType}";
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



        public IActionResult EditElection(int id)
        {
            ViewBag.ElectionId = id;
            return View(); // Create a view if needed
        }

        public IActionResult StopElection(int id)
        {
            // Add logic here
            ViewBag.ElectionId = id;
            return RedirectToAction("Elections");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteElection(int id)
        {
            var eventToDelete = await _context.EventsData.FindAsync(id);
            if (eventToDelete == null)
            {
                TempData["ErrorMessage"] = "Election not found.";
                return RedirectToAction("Elections");
            }

            try
            {
                _context.EventsData.Remove(eventToDelete);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Election deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting election");
                TempData["ErrorMessage"] = "Error deleting election: " + ex.Message;
            }

            return RedirectToAction("Elections");
        }

        // POST method to handle updating the allowed voters for an election
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllowedVoters(int ElectionId, List<string> YearLevels, List<string> Sections, List<string> Courses)
        {
            // Fetch the election data from the database
            var eventData = await _context.EventsData.FindAsync(ElectionId);
            if (eventData == null)
            {
                TempData["ErrorMessage"] = "Election not found.";
                return RedirectToAction("Elections", "AdminView");
            }

            // Validate if any selections are made for YearLevels, Sections, and Courses
            if ((YearLevels == null || YearLevels.Count == 0) ||
                (Sections == null || Sections.Count == 0) ||
                (Courses == null || Courses.Count == 0))
            {
                TempData["ErrorMessage"] = "Please select at least one Year Level, Section, and Course.";
                return RedirectToAction("Elections", "AdminView");
            }

            // Handle 'All' selection
            eventData.AllowedYearLevels = YearLevels.Contains("All") ? "All" : string.Join(",", YearLevels);
            eventData.AllowedSections = Sections.Contains("All") ? "All" : string.Join(",", Sections);
            eventData.AllowedCourses = Courses.Contains("All") ? "All" : string.Join(",", Courses);

            try
            {
                // Update the event data and save changes
                _context.Update(eventData);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Allowed voters updated successfully.";
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                TempData["ErrorMessage"] = "Failed to update allowed voters.";
            }

            // Redirect back to the elections page
            return RedirectToAction("Elections", "AdminView");
        }

        public IActionResult GetAllowedVotersData(int electionId)
        {
            // ✅ Predefined values
            var predefinedCourses = new List<string> { "BSCS", "BSIT", "BSIS", "BSDS", "BSEMC", "BSCpE" };
            var predefinedYearLevels = new List<string> { "1st Year", "2nd Year", "3rd Year", "4th Year" };
            var predefinedSections = Enumerable.Range('A', 'P' - 'A' + 1).Select(c => ((char)c).ToString()).ToList();

            // ✅ Fetch the event from the database
            var eventData = _context.EventsData.FirstOrDefault(e => e.Id == electionId);

            // ✅ Parse stored selections
            var selectedCourses = eventData?.AllowedCourses?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList() ?? new List<string>();
            var selectedYearLevels = eventData?.AllowedYearLevels?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList() ?? new List<string>();
            var selectedSections = eventData?.AllowedSections?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList() ?? new List<string>();

            // ✅ Return JSON data
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



        private List<SelectListItem> GetElectionTypes()
        {
            return Enum.GetValues(typeof(ElectionType))
                .Cast<ElectionType>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e == ElectionType.CampusGovernment ? "CSG/USG" : "Class Officer"
                })
                .ToList();
        }


    }
}
