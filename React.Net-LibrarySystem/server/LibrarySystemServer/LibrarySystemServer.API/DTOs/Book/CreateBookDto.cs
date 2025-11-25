namespace LibrarySystemServer.DTOs.Book;
public class CreateBookDto
{
    public string GoogleBookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ThumbnailUrl { get; set; }
    public int Quantity { get; set; }
}
