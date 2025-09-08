using System.Diagnostics;
using LibrarySystemApplication.Data;
using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<Member> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SignInManager<Member> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("MainLibrary", "Home");

            return View();
        }

        public IActionResult JoinUs()
        {
            return View();
        }

        [Route("/MainLibrary")]
        [Authorize(Roles = "Member,Admin,Librarian")]
        public IActionResult MainLibrary()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
