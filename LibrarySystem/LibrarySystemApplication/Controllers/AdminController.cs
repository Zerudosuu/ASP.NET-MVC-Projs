using System.Collections.Generic;
using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApplication.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
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

        var model = new Models.ViewModels.LibrarianDashBoardViewModel
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

    public IActionResult ManageBooks()
    {
        var books = _adminService.GetAllBooksAsync().Result;
        return View(books);
    }

    public async Task<IActionResult> ManageLibrarians()
    {
        var librarians = await _adminService.GetAllLibrariansAsync();
        return View(librarians);
    }
}
