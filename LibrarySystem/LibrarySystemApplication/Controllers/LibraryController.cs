using LibrarySystemApplication.Data.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystemApplication.Controllers
{

    [Authorize(Roles ="Admin")]
    public class LibraryController : Controller
    {

        private readonly ILibraryServices _libraryServices; 

        public LibraryController (ILibraryServices libraryServices)
        {
            _libraryServices = libraryServices;
        }

        [HttpPost]
        public async Task<IActionResult> Borrow(string bookId)
        {
            var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try {

                await _libraryServices.BorrowBookAsync(memberId, bookId);
                return RedirectToAction("MyBooks");
            
            }
            catch (Exception ex) {

                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Books"); 
            
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> MyBooks()
        {
            var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var barrowed = await _libraryServices.GetBorrowedBooksAsync(memberId);
            return View(barrowed);
        }

        [HttpPost]

        public async Task<IActionResult> Return (string bookId)
        {
            var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _libraryServices.ReturnBookAsync(memberId, bookId);
            return RedirectToAction("MyBooks");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
