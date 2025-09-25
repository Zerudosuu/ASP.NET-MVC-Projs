using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.Books;

namespace LibrarySystemApplication.Data.Services.Interface
{
    public interface ILibrarianService
    {
        Task<int> GetTotalMembersAsync();
        Task<int> GetTotalBooksAsync();
        Task<int> GetTotalBorrowedBooksAsync();
        Task<int> GetTotalOverdueBooksAsync();

        Task<IEnumerable<Book>> GetAllBooksAsync();

        Task<IEnumerable<Member>> GetAllMembersAsync();

        Task<IEnumerable<Borrow>> GetRecentBorrowedBooksAsync();
    }
}
