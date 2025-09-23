using LibrarySystemApplication.Data.Services.Interface;
using LibrarySystemApplication.Models.Account;
using LibrarySystemApplication.Models.Books;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LibrarySystemApplication.Data.Services
{
    public class AdminService : IAdminService
    {
        private readonly LibrarySystemAppContext _context;
        private readonly UserManager<Member> _userManager;

        public AdminService(LibrarySystemAppContext context, UserManager<Member> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalBooksAsync()
        {
            return await _context.Books.CountAsync();
        }

        public async Task<int> GetTotalBorrowedBooksAsync()
        {
            return await _context.Borrows.CountAsync();
        }

        public async Task<int> GetTotalOverdueBooksAsync()
        {
            return await _context.Borrows.CountAsync(b =>
                b.DueDate < DateTime.Now && !b.ReturnDate.HasValue
            );
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<IEnumerable<Member>> GetAllLibrariansAsync()
        {
            return await _userManager.GetUsersInRoleAsync("Librarian");
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task AddLibrarianAsync(Member librarian, string plainPassword)
        {
            if (librarian == null)
                throw new ArgumentNullException(nameof(librarian));

            // Create user with plain-text password
            var result = await _userManager.CreateAsync(librarian, plainPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(librarian, "Librarian");
            }
            else
            {
                // Handle errors (e.g., duplicate email, weak password, etc.)
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
        }

        public async Task RemoveLibrarianAsync(string librarianId)
        {
            var librarian = await _userManager.FindByIdAsync(librarianId);
            if (librarian != null)
            {
                await _userManager.RemoveFromRoleAsync(librarian, "Librarian");
                await _userManager.DeleteAsync(librarian);
            }
            else
            {
                throw new InvalidOperationException("Librarian not found.");
            }
        }

        public async Task<Member> GetLibrarianByIdAsync(string librarianId)
        {
            if (string.IsNullOrEmpty(librarianId))
                throw new ArgumentNullException(nameof(librarianId));

            return await _userManager.FindByIdAsync(librarianId);
        }

        public async Task<Member> UpdateLibrarianAsync(Member librarian)
        {
            if (librarian == null)
                throw new ArgumentNullException(nameof(librarian));

            var result = await _userManager.UpdateAsync(librarian);
            if (result.Succeeded)
            {
                return librarian;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
        }

        public async Task<IEnumerable<Member>> GetUsersByRoleASync(string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));

            return await _userManager.GetUsersInRoleAsync(role);
        }
    }
}
