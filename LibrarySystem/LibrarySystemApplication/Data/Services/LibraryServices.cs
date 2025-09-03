using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using LibrarySystemApplication.Hubs;
using System.Data;
using LibrarySystemApplication.Models.Books;


namespace LibrarySystemApplication.Data.Services
{
    public class LibraryServices : ILibraryServices
    {
        private readonly LibrarySystemAppContext _context;
        private readonly IHubContext<LibraryHub> _hubContext;

        public LibraryServices(LibrarySystemAppContext context, IHubContext<LibraryHub> hubContext)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task ApproveBorrow(string borrowId)
        {
           var borrowRecord = await _context.Borrows.Include(b => b.Book).FirstOrDefaultAsync(b => b.BorrowId == borrowId);

            if(borrowRecord == null)
                throw new InvalidOperationException("Borrow record not found");

            borrowRecord.Status = BorrowStatus.Approved;
            _context.Borrows.Update(borrowRecord);
            await _context.SaveChangesAsync();

            // Optionally, you might want to notify the member that their borrow request has been approved.
            await _hubContext.Clients.User(borrowRecord.MemberId).SendAsync("ReceiveNotification", $"Your borrow request for book '{borrowRecord.Book.Title}' has been approved.");

        }

        public async Task RejectBorrowAsync(string borrowId, string reason = null)
        {
            var borrowRecord =
                await _context.Borrows.Include(b => b.Book)
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId);

            if (borrowRecord == null)
                    throw new InvalidOperationException("Borrow record not found");
            
            borrowRecord.Status = BorrowStatus.Rejected;
            _context.Borrows.Update(borrowRecord);
            await _context.SaveChangesAsync();

            // Notify Member via SignalR
            var message = $"Your borrow request for '{borrowRecord.Book.Title}' has been rejected.";
            if (!string.IsNullOrEmpty(reason))
            {
                message += $" Reason: {reason}";
            }

            await _hubContext.Clients.User(borrowRecord.MemberId)
                .SendAsync("ReceiveNotification", message);
        }

        public async Task BorrowBookAsync(string memberId, string bookId, BorrowStatus borrowStatus)
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
        public async Task<IEnumerable<Borrow>> GetAllBookRequested(BorrowStatus? filter = null)
        {
            return await _context.Borrows.Where(b => b.Status == filter)
                .Include(b => b.Book)
                .Include(b => b.Member)
                .ToListAsync();
        }


        public async Task<Book> GetSpecificBookAsync(string bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found");
            }

            return book;
        }


    }
}
