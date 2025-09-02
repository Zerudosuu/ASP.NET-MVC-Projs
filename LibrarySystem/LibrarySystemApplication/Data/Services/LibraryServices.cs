using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibrarySystemApplication.Data.Services
{
    public class LibraryServices : ILibraryServices
    {
        private readonly LibrarySystemAppContext _context;

        public LibraryServices(LibrarySystemAppContext context)
        {
            _context = context;
        }

        public async Task BorrowBookAsync(string memberId, string bookId)
        {


            if (string.IsNullOrWhiteSpace(memberId))
                throw new ArgumentNullException(nameof(memberId));

            if (string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentNullException(nameof(bookId));



            //this code is a linq lambda expression just like mapping in JS, this returning bool 
            var alreadyBorrowed = await _context.Borrows.AnyAsync(b => b.BookId  == bookId && b.ReturnDate == null);

            if (alreadyBorrowed)
                throw new InvalidOperationException("Book is already borrowed");

            var borrow = new Borrow
            {
                MemberId = memberId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow,  // <-- important
                Status = BorrowStatus.Pending  // or whatever makes sense
            };
            await _context.Borrows.AddAsync(borrow);
            await _context.SaveChangesAsync();
        
        
        }

        public async Task<IEnumerable<Borrow>> GetBorrowedBooksAsync(string memberId)
        {

            if (string.IsNullOrWhiteSpace(memberId))
                throw new ArgumentNullException(nameof(memberId));

           return await _context.Borrows.Where(b => b.MemberId == memberId && b.ReturnDate == null)
                .Include(b => b.Book) // Include book details
                .Select(b => new Borrow
                {
                    BorrowId = b.BorrowId,
                    BookId = b.BookId,
                    Book = b.Book,
                    BorrowDate = b.BorrowDate,
                    ReturnDate = b.ReturnDate,
                    Status = b.Status
                }).ToListAsync();

        }

        public async Task ReturnBookAsync(string bookId, string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
                throw new ArgumentNullException(nameof(memberId));

            if (string.IsNullOrWhiteSpace(bookId))
                throw new ArgumentNullException(nameof(bookId));


            var borrowRecord = await _context.Borrows
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.ReturnDate == null);

            if (borrowRecord == null)
                throw new InvalidOperationException("this book is not currently borrowed by this member");

            borrowRecord.ReturnDate = DateTime.Now;

            _context.Borrows.Update(borrowRecord);
            await _context.SaveChangesAsync();
        }
    }
}
