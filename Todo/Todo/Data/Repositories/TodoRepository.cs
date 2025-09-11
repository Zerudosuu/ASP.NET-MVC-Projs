using Microsoft.EntityFrameworkCore;
using Todo.Models;

namespace Todo.Data.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoAppContext _context;

        public TodoRepository(TodoAppContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TodoTask task)
        {
            await _context.Todos.AddAsync(task);
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.Todos.FindAsync(id);
            if (task != null)
            {
                _context.Todos.Remove(task);
            }
        }

        public async Task<IEnumerable<TodoTask>> GetAllTasksAsync()
        {
            return await _context.Todos.ToListAsync();
        }

        public async Task<TodoTask?> GetByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TodoTask task)
        {
            _context.Todos.Update(task);
            await Task.CompletedTask;
        }
    }
}
