using LibrarySystemServer.Model;

namespace LibrarySystemServer.Repositories.Interfaces
{

    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(Guid id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
    
}