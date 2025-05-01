using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;

namespace ChainVote.Controllers.ViewsController
{
    [Authorize(Roles = "Admin")] // Optional: use role-based access
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
            return View();
        }

        //public async Task<IActionResult> StartElection()
        //{
                // this link to the start election button
                // and it needs at least two party list to start
                //
        //}

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
            }).ToList(); // Ensure we execute this once

            var model = new ElectionOverviewViewModel
            {
                AwaitingElections = grouped.Where(e => e.Event.Status == "Awaiting").ToList() ?? new List<ElectionSummary>(),
                InProgressElections = grouped.Where(e => e.Event.Status == "InProgress").ToList() ?? new List<ElectionSummary>(),
                CompletedElections = grouped.Where(e => e.Event.Status == "Completed").ToList() ?? new List<ElectionSummary>(),
                NewEvent = new EventsData // Also initialize NewEvent
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1)
                }
            };

            return View(model);
        }



        public IActionResult Candidates()
        {
            // Fetch election names from EventData table
            ViewBag.Elections = _context.EventsData
                .Select(e => e.EventName)
                .Distinct()
                .ToList();

             // Static positions grouped by election type
             ViewBag.PositionMap = new Dictionary<string, List<string>>()
            {
                {
                    "Campus Student Government (CSG) / University Student Government (USG)",
                    new List<string>
                    {
                        "President",
                        "Vice President for Internal Affairs",
                        "Vice President for External Affairs",
                        "Secretary",
                        "Treasurer",
                        "Auditor",
                        "Public Information Officer (PIO)",
                        "Business Manager",
                        "Senator"
                    }
                },
                {
                    "Class Officer",
                    new List<string>
                    {
                        "Class Mayor",
                        "Vice Mayor",
                        "Secretary",
                        "Treasurer",
                        "Auditor",
                        "Public Information Officer",
                        "Representative"
                    }
                }
            };

            // Fetch party lists from Organizations table
            ViewBag.PartyLists = _context.OrganizationsData
                .Select(o => o.Name)
                .Distinct()
                .ToList();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddElection(ElectionOverviewViewModel model)
        {
            // add a logic where it will pick a specific type of election and whenever
            // the user picks what type of it , it will determine the positions of the election event
            _logger.LogInformation("AddElection called at {Time}", DateTime.Now);

            // Set default values BEFORE model validation
            if (model.NewEvent != null)
            {
                model.NewEvent.Status ??= "Awaiting";
                model.NewEvent.Email ??= User.Identity?.Name ?? "admin@example.com";
                model.NewEvent.Organizations ??= "DefaultOrganization"; // or some ID/foreign key, adjust as needed
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
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    _logger.LogWarning("Validation error for {Key}: {Error}", entry.Key, error.ErrorMessage);
                }
            }

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
            // Logic for stopping election goes here
            return RedirectToAction("Elections");
        }

        // 3. Voters
        public IActionResult Voters()
        {
            return View();
        }

        public IActionResult AddVoter()
        {
            return View();
        }

        public IActionResult EditVoter(int id)
        {
            ViewBag.VoterId = id;
            return View();
        }

        // 4. Candidates
        public IActionResult MakeCandidate()
        {
            return View();
        }

        public IActionResult ReadyCandidates()
        {
            return View();
        }

        public IActionResult DeployCandidate(int id)
        {
            ViewBag.CandidateId = id;
            return View();
        }

        public IActionResult DeployedCandidates()
        {
            return View();
        }

        // 5. Contents
        public IActionResult Organizations()
        {
            return View();
        }

        public IActionResult EditPartylist(int id)
        {
            ViewBag.PartylistId = id;
            return View();
        }

        // 6. Accounts
        public IActionResult Accounts()
        {
            return View();
        }

        public IActionResult GetAccounts()
        {
            var adminRoleId = _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefault();

            var adminUserIds = _context.UserRoles
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .ToList();

            var accounts = _context.Users
                .Where(u => !adminUserIds.Contains(u.Id))
                .AsEnumerable()
                .Select(u => new
                {
                    studentId = u.StudentId,
                    fullName = u.FirstName + " " + u.LastName,
                    yearLevel = GetYearWithSuffix(u.YearLevel),
                    course = u.Course,
                    section = GetSectionWithYear(u.YearLevel, u.Section),
                    email = u.Email,
                })
                .ToList();


            return Json(new { data = accounts });
        }

        // 7. Logout
        public IActionResult Logout()
        {
            // Logic to sign out the user (optional)
            return View("Logout");
        }

        // Helper function
        private string GetYearWithSuffix(string year)
        {
            if (!int.TryParse(year, out int yearNum))
                return $"{year} Year"; // fallback if not a valid number

            string suffix = yearNum switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };

            return $"{yearNum}{suffix} Year";
        }

        private string GetSectionWithYear(string year, string section)
        {
            return $"{year}{section}";
        }

    }
}
