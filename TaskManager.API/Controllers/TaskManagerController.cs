using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Service;
using TaskManager.Data.Models;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskManagerController : ControllerBase
    {
        private readonly ILogger<TaskManagerController> _logger;
        private readonly TaskManagerClient _taskManagerClient;
        public TaskManagerController(ILogger<TaskManagerController> logger, TaskManagerClient taskManagerClient)
        {
            _logger = logger;
            _taskManagerClient = taskManagerClient;
        }

        [HttpPost("CreateTask")]
        public async Task<int> CreateTask([FromBody] TaskElement element)
        {
            ValidateParameters(element.Name, element.Description);

            _logger.LogInformation("CreateTask operation received");
            int id = await _taskManagerClient.CreateTask(element.Name, element.Description);

            return id;
        }

        private static void ValidateParameters(string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }
        }

    }
}