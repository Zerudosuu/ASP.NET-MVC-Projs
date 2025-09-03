using System.Security.Claims;
using LibrarySystemApplication.Data.Services;
using LibrarySystemApplication.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LibrarySystemApplication.Controllers;

[Authorize(Roles = "Member")]
public class MemberController : Controller
{
    private readonly LibraryServices _libraryServices;


    MemberController(LibraryServices libraryServices)
    {
        _libraryServices = libraryServices;
    }
    public IActionResult AcountBoard()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> MyBooks()
    {
        var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var barrowed = await _libraryServices.GetBorrowedBooksAsync(memberId!);
        return View(barrowed);
    }
    
    [HttpPost]
    public async Task<IActionResult> Borrow(string bookId, [FromServices] IHubContext<LibraryHub> hubContext )
    {
        var memberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        try
        {
            //create barrow with pending status
            await _libraryServices.BorrowBookAsync(memberId, bookId, BorrowStatus.Pending);


            //Notify librarians about new borrow request
            await hubContext.Clients.Group("Librarians").SendAsync("ReceiveNotification", $"New borrow request for book ID: {bookId} by member ID: {memberId}");
            return RedirectToAction("MyBooks", "Member");
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




}