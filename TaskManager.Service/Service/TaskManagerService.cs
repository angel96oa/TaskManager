using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TaskManager.gRPC.Proto;

namespace TaskManager.Service
{
    public class TaskManagerService : TaskManagerGRPCService.TaskManagerGRPCServiceBase
    {
        private readonly ILogger<TaskManagerService> _logger;

        public TaskManagerService(ILogger<TaskManagerService> logger)
        {
            _logger = logger;
        }

        public override Task<TaskCreatedMessage> CreateTask(TaskCreateMessage request, ServerCallContext context)
        {
            _logger.LogInformation("Creating task with name {Name}", request.Name);
            TaskCreatedMessage taskCreatedMessage = new TaskCreatedMessage
            {
                Id = 1
            };
            _logger.LogInformation("Task created with id {Id}", taskCreatedMessage.Id);
            return Task.FromResult(taskCreatedMessage);
        }
    }
}