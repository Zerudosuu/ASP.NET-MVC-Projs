using LibrarySystemServer.Data;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemServer.Repositories.Implementations;

public class LibrarianRepository(LibrarySystemContext context) : ILibrarianRepository{       
    
    private readonly LibrarySystemContext  _context = context;


    public async  Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task AddBookAsync(Book book)
    {
        await _context.Books.AddAsync(book);
    }

    public Task UpdateBook(Book book)
    {
        _context.Books.Update(book);
        return Task.CompletedTask;
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var tempBook = await GetBookByIdAsync(id); 
        if(tempBook != null)    
            _context.Books.Remove(tempBook);
        
    }

    public async Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync()
    {
        return await _context.BorrowRecords.ToListAsync();
    }

    public async Task<BorrowRecord> ApproveBorrowAsync(Guid borrowRecordId)
    {
        var record = await _context.BorrowRecords.FindAsync(borrowRecordId);
        if (record == null) return null;

        record.Status = BorrowStatus.Approved;
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<BorrowRecord> RejectBorrowAsync(Guid borrowRecordId)
    {
        var record = await _context.BorrowRecords.FindAsync(borrowRecordId);
        if (record == null) return null;

        record.Status = BorrowStatus.Approved;
        await _context.SaveChangesAsync();
        return record;
    }
}