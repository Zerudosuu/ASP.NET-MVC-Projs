using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;


namespace LibrarySystemServer.Repositories.Interfaces;

public interface IMemberRepository
{
    // Book operations
    Task<PageResult<Book>> GetAvailableBooksAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Book?> GetBookByIdAsync(Guid id);
    
    Task<bool> IsBookAlreadyBorrowedAsync(Guid bookId);

    // Borrow operations
    Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByMemberAsync(string memberId, bool onlyActive);
    Task<BorrowRecord?> GetBorrowRecordByIdAsync(Guid borrowId);
    Task AddBorrowRecordAsync(BorrowRecord borrowRecord);
    Task UpdateBorrowRecordAsync(BorrowRecord borrowRecord);
    
}