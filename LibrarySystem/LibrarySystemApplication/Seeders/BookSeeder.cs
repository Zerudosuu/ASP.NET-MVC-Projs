using LibrarySystemApplication.Data;
using LibrarySystemApplication.Data.Services;

namespace LibrarySystemApplication.Seeders;

public class BookSeeder
{
    private readonly LibrarySystemAppContext _context;
    private readonly BookApiService _bookApiService;

    public BookSeeder(LibrarySystemAppContext context, BookApiService bookApiService)
    {
        _context = context;
        _bookApiService = bookApiService;
    }

    public async Task SeedAsync()
    {
        if (_context.Books.Any())
            return;

        var seedQue = new List<string>
        {
            "Harry Potter",
            "Lord of the Rings",
            "Programming C#",
            "Machine Learning",
            "Data Structures",
            "Mathematics",
            "Science Fiction",
            "Philosophy",
            "History",
            "Biology",
        };

        foreach (var query in seedQue)
        {
            var books = await _bookApiService.SearchBookAsync(query, limit: 5);
            _context.Books.AddRange(books);
        }

        await _context.SaveChangesAsync();
    }
}
