using Microsoft.AspNetCore.Identity;

namespace LibrarySystemServer.Model
{
    public class Member : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; } = Gender.Other;
        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public MemberRole Role { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    
        public ICollection<BorrowRecord>? BorrowRecords { get; set; }
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
