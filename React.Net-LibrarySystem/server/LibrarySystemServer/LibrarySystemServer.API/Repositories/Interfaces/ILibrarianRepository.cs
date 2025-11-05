using LibrarySystemServer.Model;

namespace LibrarySystemServer.Repositories.Interfaces;

public interface ILibrarianRepository
{
    Task<List<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync (Guid id);
    Task AddBookAsync(Book book);
    Task UpdateBook(Book book);
    Task DeleteBookAsync(Guid id);
    // Borrow management
    Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync();
    Task<BorrowRecord> ApproveBorrowAsync(Guid borrowRecordId);
    Task<BorrowRecord> RejectBorrowAsync(Guid borrowRecordId);
}