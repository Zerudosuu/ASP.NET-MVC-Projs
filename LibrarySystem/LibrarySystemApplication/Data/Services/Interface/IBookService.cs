using LibrarySystemApplication.Models.Books;


namespace LibrarySystemApplication.Data.Services.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(string id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(string id);
        
        Task <bool> CheckIfBorrowedAsync(string memeberId, string id);

          
    }
}
