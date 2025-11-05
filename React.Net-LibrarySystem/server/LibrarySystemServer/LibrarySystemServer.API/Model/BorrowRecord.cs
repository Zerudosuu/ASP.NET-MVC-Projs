namespace LibrarySystemServer.Model
{
    public class BorrowRecord
    {
        public Guid Id { get; set; }

        // Foreign key to Book
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        // Foreign key to Member (who borrowed the book)
        public string MemberId { get; set; } = string.Empty;
        public Member Member { get; set; } = null!;

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        // Status for librarian approval & tracking
        public BorrowStatus Status { get; set; } = BorrowStatus.Pending;

        // Optional: track which librarian handled it
        public string? ApprovedById { get; set; }
        public Member? ApprovedBy { get; set; }
    }

    public enum BorrowStatus
    {
        Pending,
        Approved,
        Rejected,
        Returned,
        Overdue
    }
}