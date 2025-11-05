using LibrarySystemServer.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemServer.Data
{
    public class LibrarySystemContext : IdentityDbContext<Member>
    {
        public LibrarySystemContext(DbContextOptions<LibrarySystemContext> options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
    }
}
