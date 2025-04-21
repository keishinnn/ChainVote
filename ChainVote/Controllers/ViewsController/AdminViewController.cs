using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChainVote.Controllers.ViewsController
{
    [Authorize(Roles = "Admin")] // Optional: use role-based access
    public class AdminViewController : Controller
    {
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

        // 7. Logout
        public IActionResult Logout()
        {
            // Logic to sign out the user (optional)
            return View("Logout");
        }
    }
}
