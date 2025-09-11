using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Todo.Models;

namespace Todo.Data
{
    public class TodoAppContext : DbContext 
    {
        public TodoAppContext(DbContextOptions<TodoAppContext> options) : base(options) {  }

      public   DbSet<TodoTask> Todos { get; set; } 
        }
}
