using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.AccountViewModels;
using ChainVote.Data;
using System.Threading.Tasks;
using ChainVote.Models.Identity;
using System.Linq;

namespace ChainVote.Controllers
{
    [Authorize(Roles = "Voter")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // Edit Email Functionality
        [HttpGet]
        public IActionResult EditEmail() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(EditEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // Validate user password
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                TempData["Error"] = "Incorrect password.";
                return View(model);
            }

            // Prevent using the same email
            if (model.NewEmail.Trim().ToLower() == user.Email.Trim().ToLower())
            {
                TempData["Error"] = "You cannot use the same email address.";
                return View(model);
            }

            // Check if new email is already taken
            var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
            if (existingUser != null)
            {
                TempData["Error"] = "The email address is already registered.";
                return View(model);
            }

            // Generate and apply email change token
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Message"] = "Email updated successfully.";
                return RedirectToAction("EditEmail", "User");
            }

            // Display any errors encountered during update
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // Edit Password Functionality
        [HttpGet]
        public IActionResult EditPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // Prevent using the same password
            if (model.CurrentPassword == model.NewPassword)
            {
                TempData["Error"] = "New password cannot be the same as the current password.";
                return View(model);
            }

            // Validate password strength
            if (!IsStrongPassword(model.NewPassword))
            {
                TempData["Error"] = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
                return View(model);
            }

            // Attempt to change the password
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Message"] = "Password updated successfully.";
                return RedirectToAction("EditPassword", "User");
            }

            // Display any errors encountered during update
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // Helper method to evaluate password strength
        private bool IsStrongPassword(string password)
        {
            var hasMinimum8Chars = password.Length >= 8;
            var hasUpperChar = password.Any(char.IsUpper);
            var hasLowerChar = password.Any(char.IsLower);
            var hasNumber = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasMinimum8Chars && hasUpperChar && hasLowerChar && hasNumber && hasSpecialChar;
        }

        // Delete Account Functionality
        [HttpGet]
        public IActionResult DeleteAccount() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // Validate password before deletion
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                TempData["Error"] = "Incorrect password.";
                return View(model);
            }

            // Attempt to delete the account
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "DefaultView");
            }

            // Display any errors encountered during deletion
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // =============================
        // User Settings Page
        // =============================

        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }
    }
}
