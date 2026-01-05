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
        // 1) Local search first
        var localBooks = await _bookRepository.SearchBooksAsync(title, cancellationToken);
        if (localBooks.Any())
            return localBooks.ToDtoList();

        // 2) Fallback to Google Books
        var googleResult = await _googleBooksClien.SearchAsync(title, startIndex, maxResults, cancellationToken);
        if (googleResult.Items == null || !googleResult.Items.Any())
            return Enumerable.Empty<BookDto>();
        
        
    var booksToAdd = new List<Book>();
    foreach (var item in googleResult.Items)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Prefer checking by GoogleBookId if available
        var googleId = item.Id;
        Book? existing = null;
        if (!string.IsNullOrEmpty(googleId))
        {
            // Recommended: add a repository method GetBookByGoogleIdAsync
            existing = await _bookRepository.GetBookByGoogleIdAsync(googleId, cancellationToken);
        }

        // Fallback: check by exact title to avoid duplicates
        if (existing == null)
            existing = await _bookRepository.GetBookByTitleAsync(item.VolumeInfo.Title ?? title, cancellationToken);

        if (existing != null)
            continue; // Skip existing

        // Map Google item to your Book entity
        var entity = new Book
        {
            Id = Guid.NewGuid(),
            GoogleBookId = googleId ?? string.Empty,
            Title = item.VolumeInfo.Title ?? "Unknown",
            Author = item.VolumeInfo.Authors?.FirstOrDefault() ?? "Unknown",
            Publisher = item.VolumeInfo.Publisher ?? string.Empty,
            PublishedDate = item.VolumeInfo.PublishedDate ?? string.Empty,
            Description = item.VolumeInfo.Description ?? string.Empty,
            ThumbnailUrl = item.VolumeInfo.ImageLinks?.Thumbnail ?? string.Empty,
            PreviewLink = item.VolumeInfo.PreviewLink ?? string.Empty,
            Quantity = 1,         // Choose default rule for quantity
            TotalCopies = 1
        };

        booksToAdd.Add(entity);
    }

    if (booksToAdd.Any())
    {
        // 3) Persist as a batch
        foreach (var b in booksToAdd)
            await _bookRepository.AddBookAsync(b, cancellationToken);

        try
        {
            await _bookRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Logging recommended — do not leak exceptions to the client here.
            // If SaveChanges fails due to unique constraint race, you can swallow or handle more specifically.
            // For now: log and continue returning the google results as DTOs
            // logger.LogError(ex, "Failed to persist Google books");
        }
    }

    // Map returned items to DTOs (either from saved entities or directly)
    return booksToAdd.Select(b => b.ToDto()).ToList();
        
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
