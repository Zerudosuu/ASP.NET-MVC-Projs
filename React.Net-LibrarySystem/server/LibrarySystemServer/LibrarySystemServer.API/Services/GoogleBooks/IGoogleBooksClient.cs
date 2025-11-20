using LibrarySystemServer.Services.GoogleBooks;
namespace LibrarySystemServer.Services.GoogleBooks;



public interface IGoogleBooksClient
{
    Task<GoogleBooksDtos.GoogleBooksSearchResult> SearchAsync(string query, int startIndex, int maxResults, CancellationToken cancellationToken = default);

}