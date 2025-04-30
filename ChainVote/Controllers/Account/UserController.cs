using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.AccountViewModels;
using ChainVote.Data;
using System.Threading.Tasks;
using ChainVote.Models.Identity;

namespace ChainVote.Controllers
{
    [Authorize]
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

        // ====================
        // Edit Email
        // ====================
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

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                TempData["Error"] = "Incorrect password.";
                return View(model);
            }

            // 4. Check if the new email is the same as the current one
            if (model.NewEmail.Trim().ToLower() == user.Email.Trim().ToLower())
            {
                TempData["Error"] = "You cannot use the same email address.";
                return View(model);
            }

            // 5. Check if the new email is already in use by another account
            var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
            if (existingUser != null)
            {
                TempData["Error"] = "The email address is already registered.";
                return View(model);
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Message"] = "Email updated successfully.";
                return RedirectToAction("EditEmail", "User");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ====================
        // Edit Password
        // ====================
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

            // Check if new password is the same as current password
            if (model.CurrentPassword == model.NewPassword)
            {
                TempData["Error"] = "New password cannot be the same as the current password.";
                return View(model);
            }

            // Check password strength
            if (!IsStrongPassword(model.NewPassword))
            {
                TempData["Error"] = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Message"] = "Password updated successfully.";
                return RedirectToAction("EditPassword", "User");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // Helper method to check password strength
        private bool IsStrongPassword(string password)
        {
            var hasMinimum8Chars = password.Length >= 8;
            var hasUpperChar = password.Any(char.IsUpper);
            var hasLowerChar = password.Any(char.IsLower);
            var hasNumber = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasMinimum8Chars && hasUpperChar && hasLowerChar && hasNumber && hasSpecialChar;
        }


        // ====================
        // Delete Account
        // ====================
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

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                TempData["Error"] = "Incorrect password.";
                return View(model);
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "DefaultView");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ====================
        // Settings Page
        // ====================
        [HttpGet]
        public IActionResult Settings()
        {
            return View(); // User settings page
        }
    }
}
