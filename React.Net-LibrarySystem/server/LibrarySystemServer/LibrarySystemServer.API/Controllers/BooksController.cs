using LibrarySystemServer.Model;
using LibrarySystemServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            await bookService.AddBookAsync(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] Book book)
        {
            if (id != book.Id)
                return BadRequest();
            
            await bookService.UpdateBookAsync(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await bookService.DeleteBookAsync(id);
            return NoContent();
        }
        
    }
}