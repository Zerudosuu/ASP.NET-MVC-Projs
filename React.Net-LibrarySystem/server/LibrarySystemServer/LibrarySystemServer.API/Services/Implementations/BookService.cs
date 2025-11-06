using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Interfaces;

namespace LibrarySystemServer.Services.Implementations
{
    public class BookService(IBookRepository bookRepository) : IBookService
    {
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Where(b => b.isAvailable); 
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            if(book == null)    
                throw new ArgumentException("Book  cannot be null or empty");
            
            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book? book)
        {
            if (book!= null) {
                await _bookRepository.UpdateAsync(book);
                await _bookRepository.SaveChangesAsync();
            }
            else
                throw new ArgumentException("Book  cannot be null or empty");
        }

        public async Task DeleteBookAsync(Guid id)
        {
            await  _bookRepository.DeleteAsync(id);
            await _bookRepository.SaveChangesAsync();
         }
    }
}