using Microsoft.EntityFrameworkCore;

namespace TaskManager.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagerDbContext _context;

        public TaskRepository(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateTaskAsync(TaskElement task)
        {
            _context.TaskElements.Add(task);
            await _context.SaveChangesAsync();

            return task.id;
        }

        public async Task<IEnumerable<TaskElement>> GetAllTasksAsync()
        {
            return await _context.TaskElements.ToListAsync();
        }

        public Task<TaskElement?> GetTaskByIdAsync(int id)
        {
            return _context.TaskElements.FirstOrDefaultAsync(t => t.id == id);
        }
    }
}
