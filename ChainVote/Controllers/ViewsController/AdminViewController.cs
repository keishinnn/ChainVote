using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.DatabaseEntities;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;

namespace ChainVote.Controllers.ViewsController
{
    [Authorize(Roles = "Admin")] // Optional: use role-based access
    public class AdminViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // 2. Elections
        public IActionResult Elections()
        {
            return View();
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


        public IActionResult AddElection()
        {
            return View();
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
        public IActionResult Contents()
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
