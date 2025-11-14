using LibrarySystemServer.Model;

namespace LibrarySystemServer.Repositories.Interfaces;

public interface IMemberRepository
{
    // Book operations
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<Book?> GetBookByIdAsync(Guid id);
    
    Task<bool> IsBookAlreadyBorrowedAsync(Guid bookId);

    // Borrow operations
    Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByMemberAsync(string memberId, bool onlyActive);
    Task<BorrowRecord?> GetBorrowRecordByIdAsync(Guid borrowId);
    Task AddBorrowRecordAsync(BorrowRecord borrowRecord);
    Task UpdateBorrowRecordAsync(BorrowRecord borrowRecord);
    
}