using TaskManager.Proto;
using Grpc.Core;

namespace TaskManager.API
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
            try
            {
                _logger.LogInformation("Task creation started with name: {Name}", name);

                TaskCreatedMessage result = _client.CreateTask(new TaskCreateMessage { Name = name, Description = description, Status = Proto.Status.Open });
                return Task.FromResult(result.Id);
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "gRPC error: {StatusCode} - {Message} - Name: {Name}, Description: {Description}", rpcEx.StatusCode, rpcEx.Message, name, description);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message} - Name: {Name}, Description: {Description}", ex.Message, name, description);
                return Task.FromResult(0);
            }

        }
    }
}