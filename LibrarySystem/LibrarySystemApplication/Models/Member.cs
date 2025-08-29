using Microsoft.AspNetCore.Identity;

namespace LibrarySystemApplication.Models
{
    public class Member : IdentityUser
    {
        public string MemberId { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
   

        public List<Borrow> Borrows { get; set; } = new();
    }
}
