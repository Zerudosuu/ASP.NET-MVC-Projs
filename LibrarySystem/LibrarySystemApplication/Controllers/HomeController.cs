using System.Diagnostics;
using LibrarySystemApplication.Data;
using LibrarySystemApplication.Data.Services;
using LibrarySystemApplication.Data.Services.Interface;
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

        private readonly IBookService _bookService;

        private readonly BookApiService _bookApiService;

        public HomeController(
            ILogger<HomeController> logger,
            SignInManager<Member> signInManager,
            LibrarySystemAppContext context,
            IBookService bookService,
            BookApiService bookApiService
        )
        {
            _logger = logger;
            _signInManager = signInManager;
            _context = context;

            _bookService = bookService;
            _bookApiService = bookApiService;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("MainLibrary", "Home");
            var books = await _context.Books.Take(4).ToListAsync();
            return View(books);
        }

        [Route("/MainLibrary")]
        [Authorize(Roles = "Member")]
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

        public async Task<IActionResult> PopulateBooksInLibrary()
        {
            var books = await _bookService.GetAllAsync();
            return PartialView("_BookFeedCard", books);
        }
    }
}
