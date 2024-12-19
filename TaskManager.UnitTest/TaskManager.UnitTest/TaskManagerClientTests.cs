using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Proto;
using TaskManager.API;
using Microsoft.Extensions.Logging;
using Moq;
namespace TaskManager.UnitTest
{
    public class TaskManagerClientTests
    {
        TaskManagerGRPCService.TaskManagerGRPCServiceClient _client;

        public TaskManagerClientTests(TaskManagerGRPCService.TaskManagerGRPCServiceClient client)
        {
            _client = client;
        }
        [Fact]
        public async Task CreateTask()
        {
            // Arrange
            Mock<ILogger<TaskManagerClient>> logger = new Mock<ILogger<TaskManagerClient>>();
            
            var taskManagerClient = new TaskManagerClient(logger.Object, _client);
            var name = "Task 1";
            var description = "Description 1";
            // Act
            var result = await taskManagerClient.CreateTask(name, description);
            // Assert
            Assert.Equal(1, result);
        }
    }
}
