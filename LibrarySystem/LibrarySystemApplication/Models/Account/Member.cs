using Microsoft.AspNetCore.Identity;

namespace LibrarySystemApplication.Models.Account
{
    public class Member : IdentityUser
    {
        public string MemberId { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }

        public string? EmailAddress { get; set; }


        public string? Address { get; set; }
        public List<Borrow> Borrows { get; set; } = new();
    }
}
