using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models;
using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.Books;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemApplication.Data.Services
{
    public class LibrarianService : ILibrarianService
    {
        private readonly LibrarySystemAppContext _context;
        private readonly UserManager<Member> _userManager;

        public LibrarianService(LibrarySystemAppContext context, UserManager<Member> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            return await _userManager.GetUsersInRoleAsync("Member");
        }

        public async Task<IEnumerable<Borrow>> GetRecentBorrowedBooksAsync()
        {
            return await _context
                .Borrows.Include(b => b.Book)
                .Include(b => b.Member)
                .OrderByDescending(b => b.BorrowDate)
                .Take(5)
                .ToListAsync();
        }

        public async Task<int> GetTotalBooksAsync()
        {
            return await _context.Books.CountAsync();
        }

        public async Task<int> GetTotalBorrowedBooksAsync()
        {
            return await _context.Borrows.CountAsync();
        }

        public async Task<int> GetTotalMembersAsync()
        {
            return (await _userManager.GetUsersInRoleAsync("Member")).Count();
        }

        public async Task<int> GetTotalOverdueBooksAsync()
        {
            return await _context.Borrows.CountAsync(b =>
                b.DueDate < DateTime.Now && b.ReturnDate.HasValue == false
            );
        }
    }
}
