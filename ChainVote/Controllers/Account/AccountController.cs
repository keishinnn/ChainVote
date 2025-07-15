using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChainVote.Models.Authentication;
using System.Threading.Tasks;
using ChainVote.Models.Identity;
using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;

namespace ChainVote.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        // Constructor to inject UserManager, SignInManager, and ApplicationDbContext
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            // Try to find user by email or username (StudentId)
            ApplicationUser user = await _userManager.FindByEmailAsync(loginViewModel.Username);
            if (user == null)
                user = await _userManager.FindByNameAsync(loginViewModel.Username);

            if (user != null)
            {
                // Attempt sign-in with lockout on failure enabled
                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    loginViewModel.Password,
                    isPersistent: false,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Successful login — get user roles
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Dashboard", "AdminView");
                    }
                    else if (roles.Contains("Voter"))
                    {
                        return RedirectToAction("UserViewElections", "UserView");
                    }
                    else
                    {
                        TempData["Error"] = "No role assigned to this account.";
                        return View(loginViewModel);
                    }
                }
                else if (result.IsLockedOut)
                {
                    // User is locked out
                    var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    var remaining = lockoutEnd.HasValue
                        ? (lockoutEnd.Value.UtcDateTime - DateTime.UtcNow).TotalMinutes
                        : 0;

                    TempData["Error"] = $"Your account is locked due to multiple failed login attempts. Please try again in {Math.Ceiling(remaining)} minute(s).";
                    return View(loginViewModel);
                }
                else
                {
                    // Invalid credentials (wrong password)
                    TempData["Error"] = "Wrong credentials.";
                    return View(loginViewModel);
                }
            }

            TempData["Error"] = "User not found.";
            return View(loginViewModel);
        }


        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View(registerViewModel);

            // Check if email is already taken
            var existingUser = await _userManager.FindByEmailAsync(registerViewModel.Email);
            if (existingUser != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(registerViewModel);
            }

            // Create new user instance
            var newUser = new ApplicationUser
            {
                UserName = registerViewModel.StudentId, // StudentId is used as username
                Email = registerViewModel.Email,
                StudentId = registerViewModel.StudentId,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Course = registerViewModel.Course,
                YearLevel = registerViewModel.YearLevel,
                Section = registerViewModel.Section
            };

            // Save the new user to the database
            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (result.Succeeded)
            {
                // Assign "Voter" role to user
                await _userManager.AddToRoleAsync(newUser, "Voter");

                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Register", "Account");
            }

            // Show any validation errors
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(registerViewModel);
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "DefaultView"); // Redirect to homepage
        }

        // GET: Account/RegisterAdmin
        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View(new AdminAccount());
        }

        // POST: Account/RegisterAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(AdminAccount adminAccount)
        {
            if (!ModelState.IsValid)
                return View(adminAccount);

            // Check if email is already registered
            var existingUser = await _userManager.FindByEmailAsync(adminAccount.Email);
            if (existingUser != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(adminAccount);
            }

            // Create new admin user
            var newUser = new ApplicationUser
            {
                UserName = adminAccount.Email,
                Email = adminAccount.Email,
                StudentId = "Admin123",
                Section = "Admin",
                YearLevel = "Admin",
                Course = "Admin",
                FirstName = "Admin",
                LastName = "Admin"
            };

            var result = await _userManager.CreateAsync(newUser, adminAccount.Password);
            if (result.Succeeded)
            {
                // Assign "Admin" role to user
                await _userManager.AddToRoleAsync(newUser, "Admin");

                // Save the AdminAccount record to custom table (optional)
                _context.AdminAccount.Add(adminAccount);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Login", "Account");
            }

            // Display any errors from account creation
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(adminAccount);
        }
    }
}
