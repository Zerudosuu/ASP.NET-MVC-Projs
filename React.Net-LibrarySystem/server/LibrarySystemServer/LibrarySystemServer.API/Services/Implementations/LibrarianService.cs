using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Interfaces;

namespace LibrarySystemServer.Services.Implementations;

public class LibrarianService(ILibrarianRepository librarianRepository) : ILibrarianService
{
    private readonly ILibrarianRepository _librarianRepository = librarianRepository;
    
    
    //TODO: Need to continue LibrarianService
    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _librarianRepository.GetAllBooksAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await _librarianRepository.GetBookByIdAsync(id);
    }

    public async Task AddBookAsync(Book book)
    {
        if(book == null)
            throw new ArgumentNullException(nameof(book));
       
        await _librarianRepository.AddBookAsync(book);
        await _librarianRepository.SaveChangesAsync();
        
    }

    public async Task UpdateBookAsync(Book book)
    {
        if (book != null)
        {
            await _librarianRepository.UpdateBook(book);
            await _librarianRepository.SaveChangesAsync();
        }
    }

    public async Task DeleteBookAsync(Guid id)
    {
        await _librarianRepository.DeleteBookAsync(id);
        await _librarianRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync()
    {
      return await _librarianRepository.GetAllBorrowRecordsAsync();
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