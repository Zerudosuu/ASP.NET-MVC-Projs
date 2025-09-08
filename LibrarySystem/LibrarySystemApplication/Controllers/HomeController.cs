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

        private readonly LibrarySystemAppContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            SignInManager<Member> signInManager,
            LibrarySystemAppContext context
        )
        {
            _logger = logger;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("MainLibrary", "Home");

            var books = _context.Books.Take(3).ToList();

            return View(books);
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
