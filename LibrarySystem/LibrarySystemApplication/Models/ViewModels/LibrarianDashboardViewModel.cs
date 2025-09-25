using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.Books;

namespace LibrarySystemApplication.Models.ViewModels;

public class LibrarianDashboardViewModel
{
    public int TotalBooks { get; set; }
    public int BorrowedBooks { get; set; }
    public int OverdueBooks { get; set; }
    public int ReservedBooks { get; set; }

    public IEnumerable<Borrow> RecentBorrowedBooks { get; set; } = new List<Borrow>();

    public IEnumerable<Book> AvailableBooks { get; set; } = new List<Book>();
    public IEnumerable<Member> Members { get; set; } = new List<Member>();
}
