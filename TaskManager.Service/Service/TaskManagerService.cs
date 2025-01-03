﻿using Grpc.Core;
using TaskManager.Proto;
using TaskManager.Data;
using TaskManager.Messaging;
using RabbitMQ.Client.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Service
{
    public class TaskManagerService : TaskManagerGRPCService.TaskManagerGRPCServiceBase
    {
        private readonly ILogger<TaskManagerService> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly IRabbitMQService _rabbitMQService;

        public TaskManagerService(ILogger<TaskManagerService> logger, ITaskRepository taskRepository, IRabbitMQService rabbitMQService)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _rabbitMQService = rabbitMQService;
        }

        public async override Task<TaskIdMessage> CreateTask(TaskMessage request, ServerCallContext context)
        {
            _logger.LogInformation("Creating task with name {Name}", request.Name);
            int id = 0;
            TaskElement element = new TaskElement
            {
                Name = request.Name,
                Description = request.Description,
                status = request.Status.ToString(),
                TaskCreateDate = DateTime.UtcNow
            };

            try
            {
                id = await _taskRepository.CreateTaskAsync(element);
                _rabbitMQService.SendMessage($"Created task with id {id}\n");
            }
            catch (RabbitMQClientException ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating database");
                throw;
            }

            _logger.LogInformation("Created task with id {Id}", id);
            return new TaskIdMessage { Id = id };
        }

        public async override Task<TaskMessage> ReadTask(TaskIdMessage request, ServerCallContext context)
        {
            TaskMessage taskMessage = new TaskMessage();
            try
            {
                TaskElement result = await _taskRepository.GetTaskByIdAsync(request.Id);

                taskMessage = new TaskMessage
                {
                    Name = result.Name,
                    Description = result.Description,
                    Status = (Proto.Status)Enum.Parse(typeof(Proto.Status), result.status)
                };

                _rabbitMQService.SendMessage($"Readed task with name {taskMessage.Name}\n");
            }
            catch (RabbitMQClientException ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating database");
                throw;
            }
            return taskMessage;
        }

        public async override Task<TaskResponseEmpty> UpdateTask(TaskMessageId element, ServerCallContext context)
        {
            TaskResponseEmpty result = new TaskResponseEmpty();
            try
            {
                TaskElement taskElement = new TaskElement
                {
                    id = element.Id,
                    Name = element.Name,
                    Description= element.Description,
                    status = element.Status.ToString(),
                    finishTaskDate = DateTime.Now,
                };

                result = await _taskRepository.UpdateTaskAsync(taskElement);
                _rabbitMQService.SendMessage($"Updated task with id {element.Id}\n");
            }
            catch (RabbitMQClientException ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating database");
                throw;
            }

            return result;
        }

        public async override Task<TaskResponseEmpty> DeleteTask(TaskIdMessage id, ServerCallContext context)
        {
            TaskResponseEmpty result = new TaskResponseEmpty();
            try
            {
                result = await _taskRepository.DeleteTaskAsync(id.Id);
                _rabbitMQService.SendMessage($"Deleted task with id {id}\n");
            }
            catch (RabbitMQClientException ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating database");
                throw;
            }

            return result;
        }
    }
}