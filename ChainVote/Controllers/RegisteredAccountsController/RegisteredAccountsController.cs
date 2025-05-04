using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;

namespace ChainVote.Controllers.RegisteredAccountsController
{
    [Authorize(Roles = "Admin")]
    public class RegisteredAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisteredAccountsController(ApplicationDbContext context)
        {
            _context = context;
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
                    email = u.Email
                })
                .ToList();

            return Json(new { data = accounts });
        }

        private string GetYearWithSuffix(string year)
        {
            if (!int.TryParse(year, out int yearNum))
                return $"{year} Year";

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

        public IActionResult Voters()
        {
            return View();
        }

        public IActionResult AddVoter()
        {
            return View();
        }
    }
}
