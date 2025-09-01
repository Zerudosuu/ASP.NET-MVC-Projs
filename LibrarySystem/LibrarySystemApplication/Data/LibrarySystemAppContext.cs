using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibrarySystemApplication.Models.Books;
using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Account;

namespace LibrarySystemApplication.Data
{
    public class LibrarySystemAppContext : IdentityDbContext<Member>
    {
        public LibrarySystemAppContext(DbContextOptions<LibrarySystemAppContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Borrow> Borrows { get; set; }


    }
}
