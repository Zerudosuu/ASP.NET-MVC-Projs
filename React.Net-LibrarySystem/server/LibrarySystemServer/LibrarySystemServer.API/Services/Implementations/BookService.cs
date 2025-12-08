using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Mappers;
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

        return new PageResult<BookDto>
        {
            Items = result.Items.ToDtoList().ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByIdAsync(id, cancellationToken);
        return book?.ToDto();
    }

    public async Task<BookDto?> GetBookByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByTitleAsync(title, cancellationToken);
        return book?.ToDto();
    }

    public async Task<IEnumerable<BookDto>> SearchBookWithGoogleFallbackAsync(string title, int startIndex, int maxResults, CancellationToken cancellationToken = default)
    {
        var localBooks = await _bookRepository.SearchBooksAsync(title, cancellationToken);

        if (localBooks.Any())
            return localBooks.ToDtoList();

        var googleResult = await _googleBooksClien.SearchAsync(title, 0, 5, cancellationToken);

        if (googleResult.Items == null)
            return Enumerable.Empty<BookDto>();

        return googleResult.Items.Select(MapGoogleItemToDto).ToList();
    }

    public async Task<BookDto> AddBookAsync(CreateBookDto dto, CancellationToken cancellationToken = default)
    {
        var book = dto.ToEntity();
        await _bookRepository.AddBookAsync(book, cancellationToken);
        return book.ToDto();
    }

    public async Task<BookDto?> UpdateBookAsync(Guid id, UpdateBookDto dtoUpdate, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByIdAsync(id, cancellationToken);
        if (book == null) return null;

        book.UpdateEntity(dtoUpdate);
        await _bookRepository.UpdateBookAsync(book, cancellationToken);

        return book.ToDto();
    }

    public async Task DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _bookRepository.DeleteBookAsync(id, cancellationToken);
    }

    private static BookDto MapGoogleItemToDto(GoogleBooksDtos.GoogleBookItem item) =>
        new()
        {
            GoogleBookId = item.Id,
            Title = item.VolumeInfo.Title,
            Author = item.VolumeInfo.Authors?.FirstOrDefault() ?? "Unknown",
            ThumbnailUrl = item.VolumeInfo.ImageLinks?.Thumbnail,
            IsAvailable = true
        };
}
