using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.Books;

namespace LibrarySystemApplication.Models
{
    public class Borrow
    {
        public string BorrowId { get; set; } = Guid.NewGuid().ToString();

        // Foreign keys
        public string MemberId { get; set; }
        public Member Member { get; set; }

        public string BookId { get; set; }
        public Book Book { get; set; }

        // Extra info
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
