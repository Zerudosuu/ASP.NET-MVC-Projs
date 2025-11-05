using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;

namespace LibrarySystemServer.Services.Interfaces;

public interface ILibrarianService
{
    Task<List<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(Guid id);
    Task AddBookAsync(Book book);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(Guid id);

    Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync();
    Task<BorrowRecord> ApproveBorrowAsync(Guid borrowRecordId);
    Task<BorrowRecord> RejectBorrowAsync(Guid borrowRecordId);
}