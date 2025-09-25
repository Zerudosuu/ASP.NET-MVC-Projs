using LibrarySystemApplication.Data.Services;
using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApplication.Controllers;

[Authorize(Roles = "Librarian, Admin")]
public class LibrarianController : Controller
{
    private readonly ILibraryServices _libraryServices;

    private readonly ILibrarianService _librarianService;
    private readonly IBookService _bookService;

    public LibrarianController(
        ILibraryServices libraryServices,
        ILibrarianService librarianService,
        IBookService bookService
    )
    {
        _libraryServices = libraryServices;
        _librarianService = librarianService;
        _bookService = bookService;
    }

    #region LibraryServices

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

    public async Task<IActionResult> Requests()
    {
        var borrowquee = await _libraryServices.GetAllBookRequested(BorrowStatus.Pending);
        return View(borrowquee);
    }

    public async Task<IActionResult> ManageBooks()
    {
        var books = await _bookService.GetAllAsync();
        return View(books);
    }
    #endregion


    #region LibrarianServices

    // GET
    public async Task<IActionResult> Dashboard()
    {
        var DashboardModel = new LibrarianDashboardViewModel
        {
            TotalBooks = await _librarianService.GetTotalBooksAsync(),
            BorrowedBooks = await _librarianService.GetTotalBorrowedBooksAsync(),
            OverdueBooks = await _librarianService.GetTotalOverdueBooksAsync(),
            RecentBorrowedBooks = await _librarianService.GetRecentBorrowedBooksAsync(),

            AvailableBooks = await _bookService.GetAllAsync(),
            Members = await _librarianService.GetAllMembersAsync(),
        };

        return View(DashboardModel);
    }

    #endregion
}
