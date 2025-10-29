using System.Security.Claims;
using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.Books;
using LibrarySystemApplication.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace LibrarySystemApplication.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // GET: Books
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllAsync();
        return View(books);
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
            return NotFound();

        var book = await _bookService.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (memberId != null)
        {
            var status = await _bookService.CheckIfBorrowedAsync(memberId, id);
            ViewBag.BorrowStatus = status?.ToString() ?? "InShelf";
        }

        return PartialView("_BookPartial", book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var book = new Book
            {
                Title = model.Title,
                Isbn = model.Isbn,
                Author = model.Author,
                PublishYear = model.PublishYear,
                Publisher = model.Publisher,
                CoverUrl = model.CoverUrl,
                Language = model.Language,
                TotalCopies = model.TotalCopies,
                IsAvailable = model.IsAvailable,
                Categories = string.IsNullOrWhiteSpace(model.CategoriesInput)
                    ? new List<string>()
                    : model
                        .CategoriesInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => c.Trim())
                        .ToList(),
            };

            await _bookService.AddAsync(book);
            return RedirectToAction("ManageBooks", "Librarian");
        }

        TempData["ErrorAddingBook"] = "Failed to add book. Please check the details and try again.";
        return RedirectToAction("ManageBooks", "Librarian");
    }

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Book book)
    {
        if (id != book.BookId)
            return NotFound();

        if (ModelState.IsValid)
        {
            await _bookService.UpdateAsync(book);
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: Books/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _bookService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
