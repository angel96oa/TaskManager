using TaskManager.Proto;

namespace TaskManager.Data
{
    public interface ITaskRepository
    {
        Task<int> CreateTaskAsync(TaskElement task);
        Task<TaskElement?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskElement>> GetAllTasksAsync();
        Task<TaskResponseEmpty> UpdateTaskAsync(TaskElement task);
        Task<TaskResponseEmpty> DeleteTaskAsync(int id);
    }
}
