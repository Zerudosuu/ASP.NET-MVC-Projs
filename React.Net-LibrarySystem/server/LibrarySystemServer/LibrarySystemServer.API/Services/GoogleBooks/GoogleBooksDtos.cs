namespace LibrarySystemServer.Services.GoogleBooks;

public class GoogleBooksDtos
{
    public class GoogleBooksSearchResult
    {
        public int TotalItems { get; set; }
        public List<GoogleBookItem> Items { get; set; } = new();
    }

    public class GoogleBookItem
    {
        public string Id { get; set; }
        public GoogleBookVolumeInfo VolumeInfo { get; set; }
    }
    
    
    public class GoogleBookVolumeInfo
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public List<GoogleIndustryIdentifier> IndustryIdentifiers { get; set; }
        public GoogleImageLinks ImageLinks { get; set; }
        public string PreviewLink { get; set; }
    }

    public class GoogleIndustryIdentifier
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
    }

    public class GoogleImageLinks
    {
        public string Thumbnail { get; set; }
    }
}