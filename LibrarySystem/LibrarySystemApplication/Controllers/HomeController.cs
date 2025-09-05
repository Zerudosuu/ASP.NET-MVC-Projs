using LibrarySystemApplication.Data;
using LibrarySystemApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemApplication.Controllers
{
 
    public class HomeController : Controller
    {

        private readonly LibrarySystemAppContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, LibrarySystemAppContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }
 
        public IActionResult JoinUs()
        {
            return View();
        }

     
        public IActionResult Privacy()
        {
            return View();
        }


  
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    
    }
}
