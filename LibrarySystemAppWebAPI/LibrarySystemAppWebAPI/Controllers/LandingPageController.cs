using LibrarySystemAppWebAPI.Model;
using LibrarySystemAppWebAPI.Model.Account.Control;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace LibrarySystemAppWebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LandingPageController : ControllerBase
    {
        private readonly SignInManager<Member> _signInManager;
        private readonly UserManager<Member> _userManager;
        public LandingPageController(SignInManager<Member> signInManager, UserManager<Member> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input.");

            var result = await _signInManager.PasswordSignInAsync(
                model.Email!,
                model.Password!,
                model.RememberMe!,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email!);
                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    Message = "Login successful",
                    Email = user.Email,
                    Roles = roles
                });
            }

            return Unauthorized("Invalid email or password.");
        }




        [HttpPost("register")]
        public async Task<IActionResult> Register(Register model, string memberRole = "Member")
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var member = new Member
            {
                Name = model.UserName,
                Email = model.Email,
                UserName = model.Email,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(member, model.Password!);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(member, memberRole);

            return Ok(new { Message = "Registration successful", Role = memberRole });
        }


        // Logout method: Signs the user out and then redirects to Home.
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
            // nameof is better than a string — avoids typos and supports refactoring.
        }
    }
}

