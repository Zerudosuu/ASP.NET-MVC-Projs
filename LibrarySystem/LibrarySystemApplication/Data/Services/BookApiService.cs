using System.Globalization;
using System.Text.Json;
using LibrarySystemApplication.Models.Books;
using Microsoft.Extensions.Caching.Memory;

namespace LibrarySystemApplication.Data.Services;

public class BookApiService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private readonly LibrarySystemAppContext _context;

    public BookApiService(HttpClient client, IMemoryCache cache, LibrarySystemAppContext context)
    {
        _client = client;
        _cache = cache;
        _context = context;
    }

    public async Task<List<Book?>> SearchBookAsync(
        string query,
        int limit = 5,
        int page = 1,
        string? category = null
    )
    {
        ////check the DBFirst
        //var dbBook = _context.Books.FirstOrDefault(b => b.Title.Contains(query));
        //if (dbBook != null)
        //        return dbBook;
        ////check the memory cache
        //if (_cache.TryGetValue(query, out Book cachedBook))
        //    return cachedBook;

        var dbBooks = _context.Books.Where(b => b.Title.Contains(query)).ToList();
        if (dbBooks.Any())
            return dbBooks;

        if (_cache.TryGetValue(query, out List<Book> cachedBooks))
            return cachedBooks;

        //storing the url
        var url = $"https://openlibrary.org/search.json?title={Uri.EscapeDataString(query)}";
        //getting the response
        var response = await _client.GetStringAsync(url);

        //parsing the JSON
        using var doc = JsonDocument.Parse(response);
        var first = doc.RootElement.GetProperty("docs").EnumerateArray();
        //    return null;

        var books = new List<Book?>();

        foreach (var item in first)
        {
            var workey = item.TryGetProperty("key", out var keyProp) ? keyProp.GetString() : null;

            var book = new Book();

            book.Isbn =
                item.TryGetProperty("isbn", out var isbnArr) && isbnArr.GetArrayLength() > 0
                    ? isbnArr[0].GetString()
                    : null;

            book.Title = item.GetProperty("title").GetString();

            book.Author =
                item.TryGetProperty("author_name", out var authorArr)
                && authorArr.GetArrayLength() > 0
                    ? authorArr[0].GetString()
                    : "Unknown";

            book.CoverUrl = item.TryGetProperty("cover_i", out var coverId)
                ? $"https://covers.openlibrary.org/b/id/{coverId.GetInt32}-M.jpg"
                : null;

            book.PublishYear = item.TryGetProperty("first_publish_year", out var publishYearArr)
                ? publishYearArr.GetInt32()
                : (int?)null;
            book.Publisher =
                item.TryGetProperty("publisher", out var publisherArr)
                && publisherArr.GetArrayLength() > 0
                    ? publisherArr[0].GetString()
                    : null;
            book.Language =
                item.TryGetProperty("language", out var languageArr)
                && languageArr.GetArrayLength() > 0
                    ? languageArr[0].GetString()
                    : null;

            book.Categories = await FetchCategories(workey);

            book.OpenLibraryKey = workey;

            books.Add(book);
        }

        return books;
    }

    public async Task<List<Book?>> SeedBooksAsync(string query, int limit = 5)
    {
        return await SearchBookAsync(query, limit);
    }

    public async Task<List<Book?>> SearchBooksForUserAsync(
        string query,
        int page = 1,
        int pageSize = 20,
        string? category = null
    )
    {
        return await SearchBookAsync(query, limit: pageSize, page: page, category: category);
    }

    private async Task<List<string?>> FetchCategories(string workKey)
    {
        var subjects = new List<string>();
        try
        {
            var url = $"https://openlibrary.org{workKey}.json";
            var response = await _client.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            if (doc.RootElement.TryGetProperty("subjects", out var subJarr))
            {
                foreach (var subJ in subJarr.EnumerateArray())
                {
                    subjects.Add(subJ.GetString());
                }
            }
        }
        catch
        {
            //ignore
        }

        return subjects;
    }

    private async Task<List<string>> FetchDescription(string workKey)
    {
        var descriptions = new List<string>();
        try
        {
            var url = $"https://openlibrary.org{workKey}.json";
            var response = await _client.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            if (doc.RootElement.TryGetProperty("description", out var descJarr))
            {
                if (descJarr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var descJ in descJarr.EnumerateArray())
                    {
                        descriptions.Add(descJ.GetString());
                    }
                }
                else if (descJarr.ValueKind == JsonValueKind.String)
                {
                    descriptions.Add(descJarr.GetString());
                }
                else if (descJarr.ValueKind == JsonValueKind.Object)
                {
                    if (descJarr.TryGetProperty("value", out var valueProp))
                    {
                        descriptions.Add(valueProp.GetString());
                    }
                }
            }
        }
        catch
        {
            //ignore
        }

        return descriptions;
    }
}
