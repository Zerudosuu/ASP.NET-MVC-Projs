using LibrarySystemApplication.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApplication.ViewController
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly UserManager<Member> _userManager;
        private readonly SignInManager<Member> _signInManager;

        public SidebarViewComponent(
            UserManager<Member> userManager,
            SignInManager<Member> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_signInManager.IsSignedIn(HttpContext.User))
                return View("Guest");

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return View("Admin");
            else if (roles.Contains("Librarian"))
                return View("Librarian");
            else
                return View("Member", user); // pass user if needed
        }
    }
}
