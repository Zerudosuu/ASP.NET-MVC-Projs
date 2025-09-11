using System.Security.Claims;
using LibrarySystemApplication.Data.Services;
using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LibrarySystemApplication.Controllers;

[Authorize(Roles = "Member")]
public class MemberController : Controller
{
    private readonly ILibraryServices _libraryServices;

    public MemberController(ILibraryServices libraryServices)
    {
        _libraryServices = libraryServices;
    }

    public IActionResult AccountBoard()
    {
        return View();
    }

    public IActionResult Notification()
    {
        return View();
    }

    
    [HttpGet]
    public async Task<IActionResult> MyRequests()
    {
        var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var barrowed = await _libraryServices.GetBorrowedBooksAsync(memberId!);
        return View(barrowed);
    }
    
    [HttpGet]
    public async Task<IActionResult> MyBooks()
    {
        var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var barrowed = await _libraryServices.GetBorrowedBooksAsync(memberId!);
        var approvedBooks = barrowed.Where(b => b.Status == BorrowStatus.Approved);
        return View(approvedBooks);
    }
    

    [HttpPost]
    public async Task<IActionResult> Borrow(
        string bookId,
        [FromServices] IHubContext<LibraryHub> hubContext
    )
    {
        var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        try
        {
            //create barrow with pending status
            await _libraryServices.BorrowBookAsync(memberId, bookId, BorrowStatus.Pending);

            var book = await _libraryServices.GetSpecificBookAsync(bookId);
            var member = User.Identity.Name;

            //Notify librarians about new borrow request
            await hubContext
                .Clients.Group("Librarians")
                .SendAsync("ReceiveBorrowRequest", bookId, member);

            return RedirectToAction("MyRequests");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("AccountBoard");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Return(string bookId)
    {
        var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _libraryServices.ReturnBookAsync(bookId, memberId); // ✅ fixed order
        return RedirectToAction("MyBooks");
    }
}
