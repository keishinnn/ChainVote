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

        public IActionResult Elections()
        {
            return RedirectToAction("Elections", "EventsData");
        }

        public IActionResult EditVoter(int id)
        {
            ViewBag.VoterId = id;
            return View();
        }

        public IActionResult Organizations()
        {
            return View();
        }

        public IActionResult Candidates()
        {
            return View();
        }

        public IActionResult Accounts()
        {
            return View();
        }

        // 7. Logout
        public IActionResult Logout()
        {
            // Logic to sign out the user (optional)
            return View("Logout");
        }

    }
}
