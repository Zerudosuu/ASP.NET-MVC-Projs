using Todo.Models;

namespace Todo.Data.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoTask>> GetAllTasksAsync();
        Task<TodoTask?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TodoTask task);
        Task UpdateTaskAsync(TodoTask task);
        Task DeleteTaskAsync(int id);
    }
}
