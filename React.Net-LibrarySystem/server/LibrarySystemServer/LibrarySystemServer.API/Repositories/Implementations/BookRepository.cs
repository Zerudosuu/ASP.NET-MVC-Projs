using LibrarySystemServer.Data;
using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace LibrarySystemServer.Repositories.Implementations;

public class BookRepository : IBookRepository
{
    private readonly LibrarySystemContext _context;

    public BookRepository(LibrarySystemContext context)
    {
        _context = context;
    }

    public async Task<PageResult<Book>> GetAllBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.Where(b => b.Quantity > 0);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<Book>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<Book?> GetBookByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Title == title, cancellationToken);
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task AddBookAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var tempBook = await GetBookByIdAsync(id);
        if (tempBook != null)
        {
            _context.Books.Remove(tempBook);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .Where(b => b.Title.Contains(query) || b.Author.Contains(query))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }
}

