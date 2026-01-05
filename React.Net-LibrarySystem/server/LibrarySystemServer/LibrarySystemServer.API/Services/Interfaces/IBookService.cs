using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.DTOs.Pagination;

namespace LibrarySystemServer.Services.Interfaces;

public interface IBookService
{
    // 📌 Search + Retrieval
    Task<PageResult<BookDto>> GetAllBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookDto?> GetBookByTitleAsync(string title, CancellationToken cancellationToken = default);

    // 📌 Google Books Fallback
    
    Task<IEnumerable<BookDto>> SearchBookWithGoogleFallbackAsync(string query, int startIndex, int maxResults, CancellationToken cancellationToken = default);

    // 📌 CRUD (for Librarians only, enforce via authorization)
    Task<BookDto> AddBookAsync(CreateBookDto dto,CancellationToken cancellationToken = default);
    Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto dtoUpdate,CancellationToken cancellationToken = default);
    Task DeleteBookAsync(Guid id, CancellationToken cancellationToken = default);
}   