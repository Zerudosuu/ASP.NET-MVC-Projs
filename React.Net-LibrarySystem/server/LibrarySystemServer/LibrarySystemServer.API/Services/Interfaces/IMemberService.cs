using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;

namespace LibrarySystemServer.Services.Interfaces;

public interface IMemberService
{
    Task<PageResult<BookDto>> GetAvailableBooksAsync(int page, int size, CancellationToken cancellationToken);
    Task<IEnumerable<BorrowRecord>> GetMyBorrowedBooksAsync(string memberId, bool onlyActive);
    Task<BorrowRecord> BorrowBookAsync(string memberId, Guid bookId);
    Task<BorrowRecord> ReturnBookAsync(string memberId, Guid borrowId);
    Task CancelRequestAsync(string memberId, Guid borrowId);
}