using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Books;

namespace LibrarySystemApplication.Data.Services.Interface
{
    public interface ILibraryServices
    {
        Task BorrowBookAsync(string memberId, string bookId);
        Task<IEnumerable<Borrow>> GetBorrowedBooksAsync(string memberId);
        Task ReturnBookAsync(string bookId, string memberId);
    }
}
