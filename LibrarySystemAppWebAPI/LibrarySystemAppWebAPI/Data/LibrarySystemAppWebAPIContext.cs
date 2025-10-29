using LibrarySystemAppWebAPI.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemAppWebAPI.Data
{
    public class LibrarySystemAppWebAPIContext : IdentityDbContext<Member>
    {
        public LibrarySystemAppWebAPIContext(DbContextOptions<LibrarySystemAppWebAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
    }
}
