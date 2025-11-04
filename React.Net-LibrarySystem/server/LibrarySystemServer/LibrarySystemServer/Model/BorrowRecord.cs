namespace LibrarySystemServer.Model
{
    public class BorrowRecord
    {
        public Guid Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        // Foreign key to Member
        public string MemberId { get; set; } = string.Empty;
        public Member Member { get; set; } = null!;
    }
}
