using LibrarySystemServer.Data;
using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemServer.Repositories.Implementations;

public class MemberRepository(LibrarySystemContext context) : IMemberRepository
{
    private readonly LibrarySystemContext _context = context;


    // Repository
    public async Task<PageResult<Book>> GetAvailableBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.Where(b => b.Quantity > 0);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(b => b.Title)
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


    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<bool> IsBookAlreadyBorrowedAsync(Guid bookId)
    {
        return await _context.BorrowRecords.AnyAsync(b => b.BookId == bookId && b.ReturnDate == null && b.Status == BorrowStatus.Borrowed);
    }

    public async Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByMemberAsync(string memberId, bool onlyActive = true)
    {
        var query = _context.BorrowRecords
            .Include(b => b.Book)
            .Where(b => b.MemberId == memberId);

        if (onlyActive)
            query = query.Where(b => b.ReturnDate == null);

        return await query.ToListAsync();
    }


    public async Task<BorrowRecord?> GetBorrowRecordByIdAsync(Guid borrowId)
    {
        return await _context.BorrowRecords
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Id == borrowId);

    }

    public async Task AddBorrowRecordAsync(BorrowRecord borrowRecord)
    {
       await _context.BorrowRecords.AddAsync(borrowRecord);
       await _context.SaveChangesAsync();
    }

    public Task UpdateBorrowRecordAsync(BorrowRecord borrowRecord)
    {
        _context.BorrowRecords.Update(borrowRecord);
        return _context.SaveChangesAsync();
    }
}