using LibrarySystemApplication.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibrarySystemApplication.Controllers
{
    public class AccountController : Controller
    {
        // Inject the built-in SignInManager so we can use its methods (like Login, Logout, etc.)
        // The generic <Member> tells Identity which user class we’re working with.
        private readonly SignInManager<Member> _signInManager;

        // Inject the built-in UserManager, which handles creating, finding, and managing users.
        private readonly UserManager<Member> _userManager;


        // Dependency Injection constructor:
        // ASP.NET Core automatically provides SignInManager and UserManager when this controller is created.
        public AccountController(SignInManager<Member> signInManager, UserManager<Member> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // This GET method just shows the Login view.
        public IActionResult Login()
        {
            return View();
        }


        // The HttpPost attribute means this method is called when the Login form is submitted.
        // Task<IActionResult> means this runs asynchronously (non-blocking) and eventually returns an IActionResult.
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            // ModelState.IsValid checks if the data in "model" passes all validation rules (like [Required], [EmailAddress], etc.).
            if (ModelState.IsValid)
            {
                // PasswordSignInAsync tries to log in the user by checking Email + Password against the database.
                // Parameters: userName, password, rememberMe flag, lockoutOnFailure flag.

                var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe!, lockoutOnFailure: false);

                if (result.Succeeded) // If login is successful
                    return RedirectToAction("Index", "Home"); // Go back to home page

                // If login failed, add an error message that will show up in the view.
                ModelState.AddModelError("", "Invalid login attempt");
            }

            // If ModelState is invalid OR login failed, return the same view with the entered data.
            return View(model);
        }

        // GET method for Register page (just returns the view)
        public IActionResult Register() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                // Create a new Member object from the form input.
                Member member = new()
                {

                    Name = model.UserName,
                    Email = model.Email,
                    UserName = model.Email, // UserName is usually required in Identity
                    Address = model.Address
                };

                // Save user in database with hashed password.
                var result = await _userManager.CreateAsync(member, model.Password!);
                await _userManager.AddToRoleAsync(member, "Member");

                if (result.Succeeded)
                {
                    // Optionally sign them in automatically after registration:
                    // await _signInManager.SignInAsync(member, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // If user creation failed, display all error messages in the view.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // Logout method: Signs the user out and then redirects to Home.
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index));
            // nameof is better than a string — avoids typos and supports refactoring.
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
