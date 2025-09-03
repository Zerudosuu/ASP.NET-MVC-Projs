using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Books;

namespace LibrarySystemApplication.Data.Services.Interface
{
    public interface ILibraryServices
    {
        Task BorrowBookAsync(string memberId, string bookId, BorrowStatus borrowStatus);
        Task<IEnumerable<Borrow>> GetBorrowedBooksAsync(string memberId);
        Task ReturnBookAsync(string bookId, string memberId);

        Task ApproveBorrow(string borrowId);
        
        Task RejectBorrowAsync(string borrowId, string reason = null);
        
        Task <IEnumerable<Borrow>> GetAllBookRequested(BorrowStatus? borrowStatus);

    }
}
