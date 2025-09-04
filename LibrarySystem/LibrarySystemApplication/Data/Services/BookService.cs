using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.Books;

using Microsoft.EntityFrameworkCore;

namespace LibrarySystemApplication.Data.Services
{
    // This service implements the IBookService interface. 
    // Following the Separation of Concerns principle, 
    // we define an interface for the contract (methods) and 
    // then provide the concrete implementation here.
    public class BookService : IBookService
    {
        private readonly LibrarySystemAppContext _context;
        // _context represents the database context. 
        // It acts like a "bridge" between your code and the database. 
        // Through it, you can query, insert, update, or delete data. 

        // Constructor Dependency Injection:
        // ASP.NET Core will inject the LibrarySystemAppContext 
        // (configured in Program.cs / Startup.cs) when creating this service.
        public BookService(LibrarySystemAppContext context)
        {
            _context = context;
        }

        // AddAsync inserts a new Book record into the database.
        // Async ensures the method is non-blocking (does not freeze the app).
        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            // Adds the book entity to the "Books" DbSet (like staging it for saving).

            await _context.SaveChangesAsync();
            // Commits all staged changes to the database.
        }

        // DeleteAsync removes a book by its ID.
        public async Task DeleteAsync(string id)
        {
            var book = await _context.Books.FindAsync(id);
            // FindAsync searches the primary key directly in the database.

            if (book != null)
            {
                _context.Books.Remove(book);
                // Marks the book entity for deletion.

                await _context.SaveChangesAsync();
                // Persists the deletion.
            }
        }

        public async Task<bool> CheckIfBorrowedAsync(string memberId, string bookId)
        {
            return await _context.Borrows.AnyAsync(b =>
                b.MemberId == memberId &&
                b.BookId == bookId &&
                (b.Status == BorrowStatus.Approved || b.Status == BorrowStatus.Pending));
        }

        // GetAllAsync retrieves all books from the database.
        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
            // Converts the DbSet<Book> into a List<Book> asynchronously.
        }

        // GetByIdAsync fetches a single book by its primary key (ID).
        public async Task<Book?> GetByIdAsync(string id)
        {
            return await _context.Books.FindAsync(id);
            // Returns null if the book doesn’t exist.
        }

        // UpdateAsync modifies an existing book record.
        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            // Marks the entity as "Modified". EF Core will generate 
            // the necessary SQL UPDATE query during SaveChanges.

            await _context.SaveChangesAsync();
        }
    }
}
