using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChainVote.Controllers
{
    [Authorize(Roles = "Voter")] // Optional: use role-based access
    public class UserViewController : Controller
    {
        private readonly ILogger<UserViewController> _logger;

        public UserViewController(ILogger<UserViewController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Elections()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Vote()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }

        public IActionResult ViewProfile()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

    }
}
