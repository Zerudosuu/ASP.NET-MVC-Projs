using LibrarySystemApplication.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace LibrarySystemApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Member> _signInManager;
        private readonly UserManager<Member> _userManager;

        public AccountController(SignInManager<Member> signInManager, UserManager<Member> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Login() {



            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
               
           
                    var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe!, lockoutOnFailure: false);

                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                

                ModelState.AddModelError("", "Invalid login attempt");
            }

            return View(model);
        }

        public IActionResult Register() { return View(); }



        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid) {

                Member member = new()
                {
                    Name = model.UserName,
                    UserName = model.Email,
                    EmailAddress = model.Email,
                    Address = model.Address


                };

                

                var result = await _userManager.CreateAsync(member, model.Password!);
                


                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(member, false);
                    return RedirectToAction("Index", "Home");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

               
            }

            return View(model);
        }
        public async Task<IActionResult> Logout() {

            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index));
           
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
