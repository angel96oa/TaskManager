using TaskManager.Proto;

namespace TaskManager.API
{
    public interface ITaskManagerClient
    {
        Task<int> CreateTask(string name, string description);
        Task<TaskMessage> ReadTask(int id);
        Task UpdateTask(int id, string name, string description, Status status);
        Task DeleteTask(int id);
    }
}
