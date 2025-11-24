using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.GoogleBooks;
using LibrarySystemServer.Services.Interfaces;

namespace LibrarySystemServer.Services.Implementations;

public class BookService(IBookRepository bookRepository, IGoogleBooksClient googleBooksClient) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;
    private readonly IGoogleBooksClient _googleBooksClien = googleBooksClient;
    
    public async Task<PageResult<BookDto>> GetAllBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _bookRepository.GetAllBooksAsync(page, pageSize, cancellationToken);
        var dtoItems = result.Items.Select(MapToDto).ToList();
        
        return new PageResult<BookDto>
        {
            Items = dtoItems,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByIdAsync(id);
        if (book == null) return null;
        return MapToDto(book);
    }

    public async Task<BookDto?> GetBookByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByTitleAsync(title, cancellationToken);
        return book == null ? null : MapToDto(book);
        
    }
    

    public async Task<IEnumerable<BookDto>> SearchBookWithGoogleFallbackAsync(string title, CancellationToken cancellationToken = default)
    {
        // 1. Search DB first
        var localBooks = await _bookRepository.SearchBooksAsync(title, cancellationToken);
        if (localBooks.Any())
        {
            return localBooks.Select(MapToDto).ToList();
        }

        // 2. If not found, call Google Books API
        var googleResult = await _googleBooksClien.SearchAsync(title, 0, 5, cancellationToken); // fetch 5 results
        if (googleResult.Items == null || !googleResult.Items.Any()) return Enumerable.Empty<BookDto>();

        var dtoList = googleResult.Items.Select(item => new BookDto
        {
            GoogleBookId = item.Id,
            Title = item.VolumeInfo.Title,
            Author = item.VolumeInfo.Authors?.FirstOrDefault() ?? "Unknown",
            ThumbnailUrl = item.VolumeInfo.ImageLinks?.Thumbnail,
            IsAvailable = true
        }).ToList();

        // 3. Optionally cache them in DB
        foreach (var dto in dtoList)
        {
            var newBook = new Book
            {
                Id = Guid.NewGuid(),
                GoogleBookId = dto.GoogleBookId,
                Title = dto.Title,
                Author = dto.Author,
                ThumbnailUrl = dto.ThumbnailUrl,
                Quantity = 1
            };
            await _bookRepository.AddBookAsync(newBook);
        }

        return dtoList;
    }





    public Task<BookDto> AddBookAsync(BookDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<BookDto> UpdateBookAsync(BookDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    
    // 🔹 Manual mapping helper
    private BookDto MapToDto(Book book) => new BookDto
    {
        Id = book.Id,
        GoogleBookId = book.GoogleBookId,
        Title = book.Title,
        Author = book.Author,
        ThumbnailUrl = book.ThumbnailUrl,
        IsAvailable = book.Quantity > 0
    };
}