using Microsoft.AspNetCore.Identity;

namespace LibrarySystemServer.Model
{
    public class Member : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }
        public string ProfilePictureUrl { get; set; }

        public MemberRole Role { get; set; } = MemberRole.Member; // Default to Member
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties (relations)
        public ICollection<BorrowRecord> BorrowRecords { get; set; }
    }

    public enum MemberRole
    {
        Librarian,
        Member,
    }

    public enum Gender
    {
        Male,
        Female,
        Other,
    }
}
