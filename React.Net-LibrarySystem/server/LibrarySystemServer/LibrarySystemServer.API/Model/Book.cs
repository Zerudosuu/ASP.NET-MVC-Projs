namespace LibrarySystemServer.Model
{
    public class Book
    {
        public Guid Id { get; set; } // Local DB ID
        public string GoogleBookId { get; set; } // ID from Google Books
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double? Rating { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PreviewLink { get; set; }
        public bool isAvailable { get; set; }
        
    }
}
