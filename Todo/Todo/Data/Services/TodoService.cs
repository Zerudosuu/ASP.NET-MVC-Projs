using Todo.Data.Repositories;
using Todo.Models;

namespace Todo.Data.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;

        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task AddTaskAsync(TodoTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _todoRepository.AddAsync(task);
            await _todoRepository.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            await _todoRepository.DeleteAsync(id);
            await _todoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetAllTasksAsync()
        {
            return await _todoRepository.GetAllTasksAsync();
        }

        public async Task<TodoTask?> GetTaskByIdAsync(int id)
        {
            return await _todoRepository.GetByIdAsync(id);
        }

        public async Task UpdateTaskAsync(TodoTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _todoRepository.UpdateTaskAsync(task);
            await _todoRepository.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _todoRepository.SaveChangesAsync();
        }
    }
}
