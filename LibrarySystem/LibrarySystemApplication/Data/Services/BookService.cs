using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.Books;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemApplication.Data.Services
{
    public class BookService : IBookService
    {
        private readonly LibrarySystemAppContext _context; 
        
        public BookService(LibrarySystemAppContext context) {
        
            _context = context; 
  
        }
        
        public async Task AddAsync(Book book)
        {
           _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null) {

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            
            }
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(string id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task UpdateAsync(Book book)
        {
             _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
