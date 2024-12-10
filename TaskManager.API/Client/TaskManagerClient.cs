using Grpc.Net.Client;
using TaskManager.gRPC.Proto;
using System;
using System.Threading.Tasks;

namespace TaskManager.API.Service
{
    public class TaskManagerClient
    {
        private readonly ILogger<TaskManagerClient> _logger;
        private readonly TaskManagerGRPCService.TaskManagerGRPCServiceClient _client;

        public TaskManagerClient(ILogger<TaskManagerClient> logger, TaskManagerGRPCService.TaskManagerGRPCServiceClient client)
        {
            _logger = logger;
            _client = client;
        }

        public Task<int> CreateTask(string name, string description){
            // Create a task
            _logger.LogInformation("Task created");
            TaskCreatedMessage result = _client.CreateTask(new TaskCreateMessage { Name = name, Description = description, Status = Status.Open });
            return Task.FromResult(result.Id);
        }
    }
}