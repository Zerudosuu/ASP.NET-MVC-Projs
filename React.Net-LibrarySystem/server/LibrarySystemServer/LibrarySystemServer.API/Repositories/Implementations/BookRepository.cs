using LibrarySystemServer.Data;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemServer.Repositories.Implementations
{
    public class BookRepository(LibrarySystemContext context) : IBookRepository
    {
        private readonly LibrarySystemContext  _context = context;

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync(); 
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddAsync(Book book)
        {
             await _context.Books.AddAsync(book);
        }

        public async Task UpdateAsync(Book book)
        {
             _context.Books.Update(book);
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await GetByIdAsync(id); 
            if(book != null)
                _context.Books.Remove(book);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();        }
    }
}