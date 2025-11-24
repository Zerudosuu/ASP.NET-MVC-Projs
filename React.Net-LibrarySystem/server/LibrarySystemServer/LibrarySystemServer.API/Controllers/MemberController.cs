using System.Security.Claims;
using LibrarySystemServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemServer.Controllers
{
    [Authorize(Roles = "Member")]
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController(IMemberService memberService) : ControllerBase
    {
        private readonly IMemberService _memberService = memberService;

        //Helper to get the current authenticated member ID
        private string? MemberId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        //GET: api/member/books
        [HttpGet("books")]
        public async Task<IActionResult> GetAvailableBooks(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var books = await _memberService.GetAvailableBooksAsync(page , pageSize, cancellationToken);
            return Ok(books);
        }

        //GET: api/member/borrowed
        [HttpGet("borrowed")]
        public async Task<IActionResult> GetMyBorrowedBooks([FromQuery] bool onlyActive = true)
        {
            if (MemberId == null) return NotFound();

            var records = await _memberService.GetMyBorrowedBooksAsync(MemberId, onlyActive);
            return Ok(records);
        }

        //POST: api/member/borrow/{bookId}
        [HttpPost("borrow/{bookId:guid}")]
        public async Task<IActionResult> BorrowBook(Guid bookId)
        {
            if (MemberId == null) return Empty;
            var result = await _memberService.BorrowBookAsync(MemberId, bookId);
            return Ok(result);
        }

        // POST: api/member/return/{borrowId}

        [HttpPost("return/{borrowId:guid}")]
        public async Task<IActionResult> ReturnBook(Guid borrowId)
        {
            if (MemberId == null) return Unauthorized();
            var result = await _memberService.ReturnBookAsync(MemberId, borrowId);
            return Ok(result);
        }

        
        // DELETE: api/member/cancel/{borrowId}
        [HttpDelete("cancel/{borrowId:guid}")]
        public async Task<IActionResult> CancelBorrowRequest(Guid borrowId)
        {
            await _memberService.CancelRequestAsync(MemberId, borrowId);
            return Ok(new { message = "Borrow request cancelled." });
        }
}
}
