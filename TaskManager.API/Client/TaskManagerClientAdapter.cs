using TaskManager.Proto;

namespace TaskManager.API
{
    public class TaskManagerClientAdapter : ITaskManagerClient
    {
        private readonly TaskManagerClient _client;

        public TaskManagerClientAdapter(TaskManagerClient client)
        {
            _client = client;
        }

        public Task<int> CreateTask(string name, string description)
        {
            return _client.CreateTask(name, description);
        }

        public Task<TaskMessage> ReadTask(int id)
        {
            return _client.ReadTask(id);
        }

        public Task UpdateTask(int id, string name, string description, Status status)
        {
            return _client.UpdateTask(id, name, description, status);
        }

        public Task DeleteTask(int id)
        {
            return _client.DeleteTask(id);
        }
    }
}
