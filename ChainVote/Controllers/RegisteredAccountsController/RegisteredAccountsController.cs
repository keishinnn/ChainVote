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
                    yearLevel = FormatHelpers.GetYearWithSuffix(u.YearLevel),
                    course = u.Course,
                    section = FormatHelpers.GetSectionWithYear(u.YearLevel, u.Section),
                    email = u.Email
                })
                .ToList();

            return Json(new { data = accounts });
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
