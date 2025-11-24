using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;

namespace LibrarySystemServer.Repositories.Interfaces;

public interface IBookRepository
{
    
    Task<PageResult<Book>>GetAllBooksAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Book?> GetBookByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task <IEnumerable<Book>> SearchBooksAsync(string query, CancellationToken cancellationToken);
    
    Task <Book?> GetBookByIdAsync(Guid id);
    
    Task AddBookAsync(Book book);

    Task UpdateBookAsync(Book book);

    Task DeleteBookAsync(Guid id);
    
    Task SaveChangesAsync();

}