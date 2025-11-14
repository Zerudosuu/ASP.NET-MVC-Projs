using LibrarySystemServer.Data;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemServer.Repositories.Implementations;

public class MemberRepository(LibrarySystemContext context) : IMemberRepository
{
    private readonly LibrarySystemContext _context = context;
    
    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _context.Books.ToListAsync();
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