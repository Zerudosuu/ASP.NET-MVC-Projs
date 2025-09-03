using LibrarySystemApplication.Data.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApplication.Controllers;

[Authorize(Roles = "Librarian")]
public class LibrarianController : Controller
{
    private readonly ILibraryServices _libraryServices;
    

    public LibrarianController(ILibraryServices libraryServices)
    {
        _libraryServices = libraryServices;
    }

    [HttpPost]
    public async Task<IActionResult> ApproveBorrow(string borrowId)
    {
        await _libraryServices.ApproveBorrow(borrowId);


        return RedirectToAction("Requests");
    }

    [HttpPost]
    public async Task<IActionResult> RejectBorrow(string borrowId, string reason)
    {
        await _libraryServices.RejectBorrowAsync(borrowId, reason);
        return RedirectToAction("Requests");
    }

    public async Task <IActionResult> Requests()
    {
        var borrowquee = await _libraryServices.GetAllBookRequested(BorrowStatus.Pending);
        return View(borrowquee);
    }

    // GET
    public IActionResult Dashboard()
    {
        return View();
    }
}
