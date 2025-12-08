using LibrarySystemServer.Model;
using LibrarySystemServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemServer.Controllers
{
    [Authorize(Roles = "Librarian")]
    [ApiController]
    [Route("api/[controller]")]
    public class LibrarianController(ILibrarianService librarianService, IBookService bookService) : ControllerBase
    {
        private readonly ILibrarianService _librarianService = librarianService;
        private readonly IBookService _bookService = bookService;

        // GET: api/librarian/books
        [HttpGet("books")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _librarianService.GetAllBooksAsync();
            return Ok(books); 
        }
        
        //GET: api/librarian/books/{id}
        [HttpGet("books/{id:guid}")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var book = await _librarianService.GetBookByIdAsync(id);
            return Ok(book);
        }
        
        //GEt: api/librarian/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks(
            [FromQuery] string query,
            [FromQuery] int startIndex = 0,
            [FromQuery] int maxResults = 10,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter is required.");

            var result = await _bookService.SearchBookWithGoogleFallbackAsync(
                query,
                startIndex,
                maxResults,
                cancellationToken
            );

            return Ok(result);
        }

        
        //POST: api/librarian/books
        [HttpPost("books")]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            await _librarianService.AddBookAsync(book);
            return Ok(new { message = "Book added successfully" }); 
        }
        
        //PUT: api/librarian/books/{id}}
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] Book updatedBook)
        {
            updatedBook.Id = id;

            await _librarianService.UpdateBookAsync(updatedBook);
            return Ok(new { message = "Book updated successfully" });
        }
        
        //DELETE: api/librarian/books/{id}
        [HttpDelete("books/{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _librarianService.DeleteBookAsync(id);
            return Ok(new { message = "Book deleted successfully" });
        }
        
        
        // ──────────────────────────────────────────────────────────────
        // 📖 BORROW REQUEST MANAGEMENT
        // ──────────────────────────────────────────────────────────────

        
        // GET: api/librarian/borrow-records
        [HttpGet("borrow-records")]
        public async Task<IActionResult> GetBorrowRecords()
        {
            var records = await _librarianService.GetAllBorrowRecordsAsync();
            return Ok(records);
        }
        
        // POST: api/librarian/borrow-records/{borrowId}/approve
        [HttpPost("borrow-records/{borrowId:guid}/approve")]
        public async Task<IActionResult> ApproveBorrow(Guid borrowId)
        {
            var record = await _librarianService.ApproveBorrowAsync(borrowId);
            return Ok(record);
        }
        
        // POST: api/librarian/borrow-records/{borrowId}/reject
        [HttpPost("borrow-records/{borrowId:guid}/reject")]
        public async Task<IActionResult> RejectBorrow(Guid borrowId)
        {
            var record = await _librarianService.RejectBorrowAsync(borrowId);
            return Ok(record);
        }
    }
}
