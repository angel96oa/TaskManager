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

                TaskIdMessage result = _client.CreateTask(new TaskMessage { Name = name, Description = description, Status = Proto.Status.Open });
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

        public Task<TaskMessage> ReadTask(int id)
        {
            try
            {
                _logger.LogInformation("Reading task with Id: {Id}", id);
                TaskMessage result = _client.ReadTask(new TaskIdMessage { Id = id });
                return Task.FromResult(result);
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "gRPC error: {StatusCode} - {Message} - Id: {Id}", rpcEx.StatusCode, rpcEx.Message, id);
                return Task.FromResult(new TaskMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                return Task.FromResult(new TaskMessage());
            }
        }

        public Task UpdateTask(int id, string name, string description, Proto.Status status)
        {
            try
            {
                _logger.LogInformation("Delete task with id: {Id}", id);
                TaskResponseEmpty result = _client.UpdateTask(new TaskMessageId { Name = name, Description = description, Status = status, Id = id });
                return Task.FromResult(result);
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "gRPC error: {StatusCode} - {Message} - Id: {Id}", rpcEx.StatusCode, rpcEx.Message, id);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                return Task.FromResult(0);
            }
        }

        public Task DeleteTask(int id)
        {
            try
            {
                _logger.LogInformation("Delete task with id: {Id}", id);
                TaskResponseEmpty result = _client.DeleteTask(new TaskIdMessage { Id = id });
                return Task.FromResult(result);
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "gRPC error: {StatusCode} - {Message} - Id: {Id}", rpcEx.StatusCode, rpcEx.Message, id);
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                return Task.FromResult(0);
            }
        }
    }
}