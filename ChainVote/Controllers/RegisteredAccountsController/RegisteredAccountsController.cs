using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Utilities;

namespace ChainVote.Controllers.RegisteredAccountsController
{
    [Authorize(Roles = "Admin")]
    public class RegisteredAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisteredAccountsController(ApplicationDbContext context)
        {
            // Initialize the database context for accessing application data
            _context = context;
        }

        // GET: Retrieve all registered user accounts excluding admins, returning JSON formatted data
        public IActionResult GetAccounts()
        {
            // Find the RoleId corresponding to the "Admin" role
            var adminRoleId = _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefault();

            // Retrieve the user IDs of all users assigned the Admin role
            var adminUserIds = _context.UserRoles
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .ToList();

            // Query all users excluding those who are admins
            var accounts = _context.Users
                .Where(u => !adminUserIds.Contains(u.Id))
                .AsEnumerable()  // Switch to in-memory to use helper methods
                .Select(u => new
                {
                    studentId = u.StudentId,
                    fullName = u.FirstName + " " + u.LastName, // Combine first and last names
                    yearLevel = FormatHelpers.GetYearWithSuffix(u.YearLevel), // Format year level with suffix
                    course = u.Course,
                    section = FormatHelpers.GetSectionWithYear(u.YearLevel, u.Section), // Format section with year level
                    email = u.Email
                })
                .ToList();

            // Return the accounts data wrapped in a JSON object with key "data"
            return Json(new { data = accounts });
        }

        // GET: Display the Voters view page
        public IActionResult Voters()
        {
            // Simply render the Voters view
            return View();
        }

        // GET: Display the AddVoter view page for adding new voters
        public IActionResult AddVoter()
        {
            // Simply render the AddVoter view
            return View();
        }
    }
}
