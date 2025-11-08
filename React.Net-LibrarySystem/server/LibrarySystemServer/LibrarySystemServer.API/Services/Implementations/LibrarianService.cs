using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Interfaces;

namespace LibrarySystemServer.Services.Implementations;

public class LibrarianService(ILibrarianRepository librarianRepository) : ILibrarianService
{
    private readonly ILibrarianRepository _librarianRepository = librarianRepository;
    
    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _librarianRepository.GetAllBooksAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        var book = await _librarianRepository.GetBookByIdAsync(id); 
        if(book != null)
            return book;
        else 
            throw new ArgumentNullException(nameof(book));
    }

    public async Task AddBookAsync(Book book)
    {
        //validation logic
        if(string.IsNullOrWhiteSpace(book.Title))
            throw new ArgumentNullException(nameof(book.Title));

        if (book.Quantity < 1 )
        {
            throw new ArgumentOutOfRangeException(nameof(book.Quantity));
        }
        
        var existingBooks  = await _librarianRepository.GetAllBooksAsync();
        
        if(existingBooks.Any(b => b.Title.Equals(book.Title, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A book with this title already exist");

        await _librarianRepository.AddBookAsync(book);
    }

    public async Task UpdateBookAsync(Book book)
    {
        var existingBook = await _librarianRepository.GetBookByIdAsync(book.Id);
        
        if(existingBook == null)
            throw new KeyNotFoundException("Book with this id does not exist");
        
        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Publisher = book.Publisher;
        existingBook.PublishedDate = book.PublishedDate;
        existingBook.Description = book.Description;
        existingBook.Category = book.Category;
        existingBook.Rating = book.Rating;
        existingBook.ThumbnailUrl = book.ThumbnailUrl;
        existingBook.PreviewLink = book.PreviewLink;

    }

    public async Task DeleteBookAsync(Guid id)
    {
        var book = await _librarianRepository.GetBookByIdAsync(id);
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        if (book.IsAvailable == false)
            throw new InvalidOperationException("Book is not available");
        

        await _librarianRepository.DeleteBookAsync(id);
    }

    //BorrowLogic
    public async Task<IEnumerable<BorrowRecord>> GetAllBorrowRecordsAsync()
    {
      return await _librarianRepository.GetAllBorrowRecordsAsync();
    }

    public async Task<BorrowRecord> ApproveBorrowAsync(Guid borrowRecordId)
    {
        var record = await _librarianRepository.ApproveBorrowAsync(borrowRecordId);
        if(record == null)
                throw new KeyNotFoundException("Borrow record not found");

        var book = record.Book;
        if(book.Quantity <= 0)
                throw new InvalidOperationException("Borrow record is not available");

        book.Quantity -= 1;
        await _librarianRepository.UpdateBook(book);
        return record; 
    }

    public async Task<BorrowRecord> RejectBorrowAsync(Guid borrowRecordId)
    {
        var record = await _librarianRepository.RejectBorrowAsync(borrowRecordId);
        if (record == null)
            throw new KeyNotFoundException("Borrow record not found");

        if (record.Book.Quantity < record.Book.TotalCopies)
        {
            record.Book.Quantity += 1;
            await _librarianRepository.UpdateBook(record.Book); 
        }

        return record;
    }
}