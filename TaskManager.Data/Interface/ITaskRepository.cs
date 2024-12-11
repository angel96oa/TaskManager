namespace TaskManager.Data
{
    public interface ITaskRepository
    {
        Task<int> CreateTaskAsync(TaskElement task);
        Task<TaskElement?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskElement>> GetAllTasksAsync();
    }
}
