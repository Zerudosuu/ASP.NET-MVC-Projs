using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.Model;

namespace LibrarySystemServer.Mappers;

public static class BookMapper
{
    // Entity → DTO
    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            GoogleBookId = book.GoogleBookId,
            Title = book.Title,
            Author = book.Author,
            ThumbnailUrl = book.ThumbnailUrl,
            IsAvailable = book.IsAvailable
        };
    }

    // List<Entity> → List<DTO>
    public static IEnumerable<BookDto> ToDtoList(this IEnumerable<Book> books)
    {
        return books.Select(b => b.ToDto());
    }

    // CreateDto → Entity
    public static Book ToEntity(this CreateBookDto dto)
    {
        return new Book
        {
            Id = Guid.NewGuid(),
            GoogleBookId = dto.GoogleBookId,
            Title = dto.Title,
            Author = dto.Author,
            ThumbnailUrl = dto.ThumbnailUrl,
            Quantity = dto.Quantity,
            TotalCopies = dto.Quantity
        };
    }

    // UpdateDto → Existing Entity
    public static void UpdateEntity(this Book book, UpdateBookDto dto)
    {
        book.Title = dto.Title;
        book.Author = dto.Author;
        book.ThumbnailUrl = dto.ThumbnailUrl;
        book.Quantity = dto.Quantity;
    }
}