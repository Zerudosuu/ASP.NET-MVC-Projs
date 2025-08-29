using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Books;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemApplication.Data.Services
{
    public class LibraryServices : ILibraryServices
    {
        private readonly LibrarySystemAppContext _context;

        public LibraryServices(LibrarySystemAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(string id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> BorrowBookAsync(string memberId, string bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null || !book.IsAvailable)
                return false;

            var borrow = new Borrow
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowDate = DateTime.Now,
            };

            book.IsAvailable = false;

            _context.Borrows.Add(borrow);
            _context.Books.Update(book);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
