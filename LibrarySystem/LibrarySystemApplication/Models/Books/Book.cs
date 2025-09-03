using System.ComponentModel.DataAnnotations;

namespace LibrarySystemApplication.Models.Books
{
    public class Book
    {
        public string BookId { get; set; } 

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Author { get; set; }

        public string Category { get; set; }

        public DateTime PublishedDate { get; set; }
        public string CoverImageUrl { get; set; }

        // New field
        public bool IsAvailable { get; set; } = true;

        public int TotalCopies { get; set; } = 1;

    }
}
