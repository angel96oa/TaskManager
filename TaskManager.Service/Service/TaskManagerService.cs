using Grpc.Core;
using TaskManager.gRPC.Proto;
using TaskManager.Data;
using TaskManager.Messaging;

namespace TaskManager.Service
{
    public class TaskManagerService : TaskManagerGRPCService.TaskManagerGRPCServiceBase
    {
        private readonly ILogger<TaskManagerService> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly RabbitMQService _rabbitMQService;

        public TaskManagerService(ILogger<TaskManagerService> logger, ITaskRepository taskRepository, RabbitMQService rabbitMQService)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _rabbitMQService = rabbitMQService;
        }

        public async override Task<TaskCreatedMessage> CreateTask(TaskCreateMessage request, ServerCallContext context)
        {
            _logger.LogInformation("Creating task with name {Name}", request.Name);
            TaskElement element = new TaskElement
            {
                Name = request.Name,
                Description = request.Description,
                status = request.Status.ToString()
            };

            int id = await _taskRepository.CreateTaskAsync(element);

            _rabbitMQService.SendMessage("Created new registry on database");

            return new TaskCreatedMessage { Id = id };
        }
    }
}