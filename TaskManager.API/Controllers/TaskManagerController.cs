using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Proto;

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

        [HttpGet("ReadTask")]
        public async Task<ActionResult<TaskMessage>> ReadTask(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Not valid id");
            }

            try
            {
                _logger.LogInformation("ReadTask operation received");
                return await _taskManagerClient.ReadTask(id);

            } catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("UpdateTask")]
        public async Task<ActionResult> UpdateTask([FromBody] TaskMessage element, int id)
        {
            if (string.IsNullOrEmpty(element.Name) || string.IsNullOrEmpty(element.Description))
            {
                return BadRequest("Name and Description cannot be empty");
            }

            try
            {
                _logger.LogInformation("UpdateTask operation received");
                await _taskManagerClient.UpdateTask(id, element.Name, element.Description, element.Status);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("DeleteTask")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Not valid id");
            }

            try
            {
                _logger.LogInformation("DeleteTask operation received");
                await _taskManagerClient.DeleteTask(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}