using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.Books;

namespace LibrarySystemApplication.Data.Services.Interface
{
    public interface IAdminService
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalBooksAsync();
        Task<int> GetTotalBorrowedBooksAsync();
        Task<int> GetTotalOverdueBooksAsync();

        Task<IEnumerable<Book>> GetAllBooksAsync();

        Task<IEnumerable<Member>> GetAllLibrariansAsync();

        Task<IEnumerable<Member>> GetAllMembersAsync();

        Task<IEnumerable<Member>> GetUsersByRoleASync(string Role);

        Task<Member> GetLibrarianByIdAsync(string librarianId);

        Task<Member> UpdateLibrarianAsync(Member librarian);
        Task AddLibrarianAsync(Member librarian, string password);
        Task RemoveLibrarianAsync(string librarianId);
    }
}
