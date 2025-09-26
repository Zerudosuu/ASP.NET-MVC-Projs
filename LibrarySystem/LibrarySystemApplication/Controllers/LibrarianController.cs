using LibrarySystemApplication.Data.Services;
using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IActionResult> ManageBooks(
        string search = "",
        int page = 1,
        int pageSize = 10
    )
    {
        var query = _libraryServices.GetAvailableBooksAsync().Result; // IQueryable<Book>

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowered = search.ToLower();
            query = query.Where(b =>
                b.Title.ToLower().Contains(lowered) || b.Author.ToLower().Contains(lowered)
            );
        }

        var totalBooks = await query.CountAsync();

        // Ensure page doesn't go out of range
        var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
        if (page < 1)
            page = 1;
        if (page > totalPages)
            page = totalPages;

        var pagedBooks = await query
            .OrderBy(b => b.Title) // always have stable order
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = page;
        ViewBag.Search = search;

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return PartialView("_BookTablePartial", pagedBooks);
        }

        return View(pagedBooks);
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
