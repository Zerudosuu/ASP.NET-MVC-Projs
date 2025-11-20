using System.Text.Json;
using LibrarySystemServer.Services.GoogleBooks;

namespace LibrarySystemServer.Services.GoogleBooks;


public class GoogleBooksClient : IGoogleBooksClient
{
    
    private readonly HttpClient _client;
    private readonly string _apiKey;
    
    public GoogleBooksClient(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _apiKey = configuration["GoogleBooks:ApiKey"] ?? throw new ArgumentNullException("GoogleBooks API key is not configured.");
    }
    
    
    public async Task<GoogleBooksDtos.GoogleBooksSearchResult> SearchAsync(string query, int startIndex, int maxResults, CancellationToken cancellationToken = default)
    {
        var url = $"volumes?q={Uri.EscapeDataString(query)}&startIndex={startIndex}&maxResults={maxResults}";
       if (!string.IsNullOrEmpty(_apiKey))
           url += $"&key={_apiKey}";

       var response = await _client.GetAsync(url, cancellationToken);
       response.EnsureSuccessStatusCode();

       var stream = await response.Content.ReadAsStreamAsync();
       var result = await JsonSerializer.DeserializeAsync<GoogleBooksDtos.GoogleBooksSearchResult>(stream, new JsonSerializerOptions
       {
           PropertyNameCaseInsensitive = true
       }, cancellationToken);

       return result ?? new GoogleBooksDtos.GoogleBooksSearchResult();
    }
}