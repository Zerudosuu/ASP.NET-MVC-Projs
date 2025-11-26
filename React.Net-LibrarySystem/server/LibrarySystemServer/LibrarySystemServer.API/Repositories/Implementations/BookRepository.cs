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
        var query = _context.Books.AsNoTracking();

        var totalItems = await query.CountAsync(cancellationToken);
        
        var items = await query.
            OrderBy(b => b.Title)
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
        return await _context.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Title == title, cancellationToken);
    }

    public async Task<Book?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
       return await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddBookAsync(Book book, CancellationToken cancellationToken = default)
    {
        await _context.Books.AddAsync(book, cancellationToken);
  
    }

    public Task UpdateBookAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Update(book);
        return Task.CompletedTask;
    }

    public async Task DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tempBook = await GetBookByIdAsync(id, cancellationToken);
        if (tempBook != null)
        {
            _context.Books.Remove(tempBook);
        }
    }

    public async Task SaveChangesAsync(CancellationToken  cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _context.Books.AsNoTracking()
            .Where(b => b.Title.Contains(query) || b.Author.Contains(query))
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }
}

