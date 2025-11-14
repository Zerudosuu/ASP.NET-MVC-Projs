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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BorrowRecord>()
       .HasOne(br => br.Member)
       .WithMany(m => m.BorrowRecords)
       .HasForeignKey(br => br.MemberId)
       .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.ApprovedBy)
                .WithMany()
                .HasForeignKey(br => br.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
