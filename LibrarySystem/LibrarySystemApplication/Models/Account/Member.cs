using Microsoft.AspNetCore.Identity;

namespace LibrarySystemApplication.Models.Account
{
    public class Member : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public List<Borrow> Borrows { get; set; } = new();
    }
}
