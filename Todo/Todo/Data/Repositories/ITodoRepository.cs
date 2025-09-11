using Todo.Models;

namespace Todo.Data.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoTask>> GetAllTasksAsync();
        Task<TodoTask?> GetByIdAsync(int id);
        Task AddAsync(TodoTask task);
        Task UpdateTaskAsync(TodoTask task);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
