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

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            // Allow login via Email or Student ID
            ApplicationUser user = await _userManager.FindByEmailAsync(loginViewModel.Username);
            if (user == null)
                user = await _userManager.FindByNameAsync(loginViewModel.Username); // Assumes StudentId is set as username

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        // Check the user's role
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Dashboard", "AdminView"); // Redirect admin
                        }
                        else if (roles.Contains("Voter"))
                        {
                            return RedirectToAction("Index", "UserView"); // Redirect voter
                        }
                        else
                        {
                            TempData["Error"] = "No role assigned to this account.";
                            return View(loginViewModel);
                        }
                    }
                }

                TempData["Error"] = "Wrong credentials.";
                return View(loginViewModel);
            }

            TempData["Error"] = "User not found.";
            return View(loginViewModel);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View(registerViewModel);

            var existingUser = await _userManager.FindByEmailAsync(registerViewModel.Email);
            if (existingUser != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(registerViewModel);
            }

            var newUser = new ApplicationUser
            {
                UserName = registerViewModel.StudentId, // Login via StudentId
                Email = registerViewModel.Email,
                StudentId = registerViewModel.StudentId,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Course = registerViewModel.Course,
                YearLevel = registerViewModel.YearLevel,
                Section = registerViewModel.Section
            };

            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Voter");
                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Register", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(registerViewModel);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "DefaultView");
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View(new AdminAccount());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(AdminAccount adminAccount)
        {
            if (!ModelState.IsValid)
                return View(adminAccount);

            var existingUser = await _userManager.FindByEmailAsync(adminAccount.Email);
            if (existingUser != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(adminAccount);
            }

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
                await _userManager.AddToRoleAsync(newUser, "Admin");

                // Optional: Save AdminAccount record too
                _context.AdminAccount.Add(adminAccount);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(adminAccount);
        }


    }
}
