using System.Collections.Generic;
using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemApplication.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILibraryServices _libraryServices;

    public AdminController(IAdminService adminService, ILibraryServices libraryServices)
    {
        _adminService = adminService;
        _libraryServices = libraryServices;
    }

    public async Task<IActionResult> Dashboard(string role = "All")
    {
        IEnumerable<Member> users;

        if (role == "Librarian")
            users = await _adminService.GetAllLibrariansAsync();
        else if (role == "Member")
            users = await _adminService.GetUsersByRoleASync("Member");
        else
            users = await _adminService.GetAllMembersAsync();

        var model = new AdminDashboardViewModel
        {
            UserCount = await _adminService.GetTotalUsersAsync(),
            BookCount = await _adminService.GetTotalBooksAsync(),
            BorrowedCount = await _adminService.GetTotalBorrowedBooksAsync(),
            OverdueCount = await _adminService.GetTotalOverdueBooksAsync(),

            Members = users,
            SelectedRole = role,
        };

        return View(model);
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

    public async Task<IActionResult> ManageLibrarians()
    {
        var librarians = await _adminService.GetAllLibrariansAsync();
        return View(librarians);
    }
}
