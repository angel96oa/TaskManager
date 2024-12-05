using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.API.Models;

namespace TaskManager.API.Service
{
    public class TaskManagerService
    {
        private readonly ILogger<TaskManagerService> _logger;

        public TaskManagerService(ILogger<TaskManagerService> logger)
        {
            _logger = logger;
        }

        public Task<Guid> CreateTask(string name, string description){
            // Create a task
            _logger.LogInformation("Task created");
            return Task.FromResult(Guid.NewGuid());
        }
    }
}