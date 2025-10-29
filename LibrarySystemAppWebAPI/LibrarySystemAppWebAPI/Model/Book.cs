using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystemAppWebAPI.Model
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BookId { get; set; } // Your DB PK

        public string Title { get; set; }

        public string? Isbn { get; set; }
        public string? Author { get; set; }
        public string? CoverUrl { get; set; }
        public int? PublishYear { get; set; }
        public string? Publisher { get; set; }

        public string? OpenLibraryKey { get; set; } // e.g. "/works/OL45883W"

        public List<string> Categories { get; set; } = new List<string>();
        public string? Language { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdated { get; set; }

        public bool IsAvailable { get; set; } = true;
        public int TotalCopies { get; set; } = 4;
    }
}
