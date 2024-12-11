using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;

namespace TaskManager.API
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
        public async Task<ActionResult<int>> CreateTask([FromBody] TaskElement element)
        {
            if (string.IsNullOrEmpty(element.Name) || string.IsNullOrEmpty(element.Description))
            {
                return BadRequest("Name and Description cannot be empty");
            }

            try
            {
                _logger.LogInformation("CreateTask operation received");
                int id = await _taskManagerClient.CreateTask(element.Name, element.Description);

                if(id == 0)
                {
                    return StatusCode(500, "Internal Server Error");
                }

                return Ok(id);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}