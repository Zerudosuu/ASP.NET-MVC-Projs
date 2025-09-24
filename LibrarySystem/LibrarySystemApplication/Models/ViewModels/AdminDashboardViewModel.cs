using LibrarySystemApplication.Models.Account;

namespace LibrarySystemApplication.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int UserCount { get; set; }
    public int BookCount { get; set; }
    public int BorrowedCount { get; set; }
    public int OverdueCount { get; set; }

    public IEnumerable<Member> Members { get; set; } = new List<Member>();

    public string SelectedRole { get; set; } = "All";
}
