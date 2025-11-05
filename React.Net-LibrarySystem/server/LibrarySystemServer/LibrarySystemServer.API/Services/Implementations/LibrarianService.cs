using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Interfaces;

namespace LibrarySystemServer.Services.Implementations;

public class LibrarianService(ILibrarianRepository librarianRepository) : ILibrarianService
{
    private readonly ILibrarianRepository _librarianRepository = librarianRepository;
    
    public Task<List<Book>> GetAllBooksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Book?> GetBookByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AddBookAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBookAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBookAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BorrowRecord> ApproveBorrowAsync(Guid borrowRecordId)
    {
        throw new NotImplementedException();
    }

    public Task<BorrowRecord> RejectBorrowAsync(Guid borrowRecordId)
    {
        throw new NotImplementedException();
    }
}