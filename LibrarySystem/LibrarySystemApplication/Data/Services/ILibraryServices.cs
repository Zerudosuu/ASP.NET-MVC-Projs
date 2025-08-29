using LibrarySystemApplication.Models.Books;

namespace LibrarySystemApplication.Data.Services
{
    public interface ILibraryServices
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(string id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(string id);
        Task<bool> BorrowBookAsync(string memberId, string bookId);
    }
}
