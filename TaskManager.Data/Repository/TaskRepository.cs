using Microsoft.EntityFrameworkCore;
using TaskManager.Proto;

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

        public Task<TaskResponseEmpty> UpdateTaskAsync(TaskElement task)
        {
            var result = _context.TaskElements.FirstOrDefault(_ => _.id == task.id);
            if (result != null)
            {
                result.Name = task.Name;
                result.Description = task.Description;
                result.status = task.status;
            }
            _context.SaveChanges();
            return Task.FromResult(new TaskResponseEmpty());
        }

        public Task<TaskResponseEmpty> DeleteTaskAsync(int id)
        {
            var result = _context.TaskElements.FirstOrDefault(_ => _.id == id);
            if (result != null)
            {
                _context.TaskElements.Remove(result);
            }
            _context.SaveChanges();
            return Task.FromResult(new TaskResponseEmpty());
        }
    }
}
