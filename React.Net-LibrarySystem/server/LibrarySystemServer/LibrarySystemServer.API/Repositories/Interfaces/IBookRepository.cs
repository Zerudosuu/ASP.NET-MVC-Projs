using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;

namespace LibrarySystemServer.Repositories.Interfaces;

public interface IBookRepository
{
    
    Task<PageResult<Book>>GetAllBooksAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Book?> GetBookByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Book?> GetBookByGoogleIdAsync(string googleBookId, CancellationToken cancellationToken = default);
    Task <IEnumerable<Book>> SearchBooksAsync(string query, CancellationToken cancellationToken);
    
    Task <Book?> GetBookByIdAsync(Guid id,CancellationToken cancellationToken );
    
    Task AddBookAsync(Book book, CancellationToken cancellationToken);

    Task UpdateBookAsync(Book book, CancellationToken cancellationToken);

    Task DeleteBookAsync(Guid id, CancellationToken cancellationToken);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);

}