namespace LibrarySystemServer.DTOs.Book;

public class UpdateBookDto
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ThumbnailUrl { get; set; }
    public int Quantity { get; set; }
}
