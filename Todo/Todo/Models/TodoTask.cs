using System.ComponentModel.DataAnnotations;

namespace Todo.Models
{
    public class TodoTask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public StatusTask Status { get; set; } = StatusTask.Ongoing;
    }

    public enum StatusTask
    {
        Ongoing,
        Overdue,
        Completed,
    }
}
