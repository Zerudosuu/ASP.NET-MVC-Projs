using Microsoft.AspNetCore.Identity;

namespace LibrarySystemAppWebAPI.Model
{
    public class Member : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public List<Borrow> Borrows { get; set; } = new();
    }
}
