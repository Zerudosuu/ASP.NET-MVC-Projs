namespace LibrarySystemServer.DTOs.Book;

public class BookDto
{
    public Guid Id { get; set; }
    public string GoogleBookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ThumbnailUrl { get; set; }
    public bool IsAvailable { get; set; }
}