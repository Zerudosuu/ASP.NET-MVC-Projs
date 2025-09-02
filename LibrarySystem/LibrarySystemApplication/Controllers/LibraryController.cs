using LibrarySystemApplication.Data.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystemApplication.Controllers
{


    public class LibraryController : Controller
    {

        private readonly ILibraryServices _libraryServices;

        public LibraryController(ILibraryServices libraryServices)
        {
            _libraryServices = libraryServices;
        }

        [HttpPost]
        public async Task<IActionResult> Borrow(string bookId)
        {
            var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                await _libraryServices.BorrowBookAsync(memberId, bookId);
                return RedirectToAction("MyBooks");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Books");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Return(string bookId)
        {
            var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _libraryServices.ReturnBookAsync(bookId, memberId); // ✅ fixed order
            return RedirectToAction("MyBooks");
        }


        [HttpGet]
        public async Task<IActionResult> MyBooks()
        {
            var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var barrowed = await _libraryServices.GetBorrowedBooksAsync(memberId);
           

            return View(barrowed);
        }

   
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
