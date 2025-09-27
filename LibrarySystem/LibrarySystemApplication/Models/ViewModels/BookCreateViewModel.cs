namespace LibrarySystemApplication.Models.ViewModels
{
    public class BookCreateViewModel
    {
        public string Title { get; set; }
        public string? Isbn { get; set; }
        public string? Author { get; set; }
        public int? PublishYear { get; set; }
        public string? Publisher { get; set; }
        public string? CoverUrl { get; set; }
        public string? Language { get; set; }
        public int TotalCopies { get; set; } = 1;
        public bool IsAvailable { get; set; } = true;

        // Comma-separated input
        public string? CategoriesInput { get; set; }
    }
}
