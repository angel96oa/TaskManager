using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Models;
using TaskManager.API.Service;
namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskManagerController : ControllerBase
    {
        private readonly ILogger<TaskManagerController> _logger;
        private readonly TaskManagerService _taskManagerService;
        public TaskManagerController(ILogger<TaskManagerController> logger, TaskManagerService taskManagerService)
        {
            _logger = logger;
            _taskManagerService = taskManagerService;
        }

        [HttpPost("CreateTask")]
        public async Task<Guid> CreateTask(string name, string description)
        {
            ValidateParameters(name, description);

            _logger.LogInformation("CreateTask operation received");
            Guid id = await _taskManagerService.CreateTask(name, description);

            return id;
        }

        private static void ValidateParameters(string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            };
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }
        }

    }
}