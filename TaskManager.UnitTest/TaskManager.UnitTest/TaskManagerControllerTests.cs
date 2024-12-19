using TaskManager.Messaging;
using TaskManager.Data;
using TaskManager.Service;
using TaskManager.API;
using Moq;
using Microsoft.Extensions.Logging;
using AutoFixture;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Proto;

namespace TaskManager.UnitTest
{
    public class TaskManagerControllerTests
    {
        [Fact]
        public async Task CreateTaskOK()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskElement
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            mockClient.Setup(x => x.CreateTask("Test Task", "Test Description"))
                .ReturnsAsync(1);

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.CreateTask(taskElement);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task CreateTaskBadRequest()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskElement
            {
                Name = null,
                Description = "Test Description"
            };

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.CreateTask(taskElement);

            // Assert
            var badrequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badrequestResult.StatusCode);
        }

        [Fact]
        public async Task CreateTaskId0()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskElement
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            mockClient.Setup(x => x.CreateTask("Test Task", "Test Description"))
                .ReturnsAsync(0);

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.CreateTask(taskElement);

            // Assert
            var badrequestResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, badrequestResult.StatusCode);
        }

        [Fact]
        public async Task CreateTaskStatus500()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskElement
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            mockClient.Setup(x => x.CreateTask("Test Task", "Test Description"))
                .ThrowsAsync(new Exception());

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.CreateTask(taskElement);

            // Assert
            var badrequestResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, badrequestResult.StatusCode);
        }

        [Fact]
        public async Task ReadTaskOK()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();
            int id = 1;

            TaskMessage taskElement = new TaskMessage
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            var task = mockClient.Setup(x => x.ReadTask(id))
                .ReturnsAsync(taskElement);

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.ReadTask(id);

            // Assert
            var okResult = Assert.IsType<TaskMessage>(result.Value);
            Assert.Equal(taskElement.Name, okResult.Name);
        }

        [Fact]
        public async Task ReadTask0()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();
            int id = 0;

            TaskMessage taskElement = new TaskMessage
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            var task = mockClient.Setup(x => x.ReadTask(id))
                .ReturnsAsync(taskElement);

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.ReadTask(id);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task ReadTaskError500()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();
            int id = 1;

            TaskMessage taskElement = new TaskMessage
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            var task = mockClient.Setup(x => x.ReadTask(id))
                .ThrowsAsync(new Exception());

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.ReadTask(id);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateTaskOK()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskMessage
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            mockClient.Setup(x => x.UpdateTask(1, "Test Task", "Test Description", Status.InProgress));

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.UpdateTask(taskElement, 1);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateTaskBadRequest()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskMessage
            {
                Name = "",
                Description = "Test Description"
            };

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.UpdateTask(taskElement, 1);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateTaskError500()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskMessage
            {
                Name = "Element Name",
                Description = "Test Description"
            };

            mockClient
                .Setup(client => client.UpdateTask(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Status>()))
                .ThrowsAsync(new Exception("Test exception"));


            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.UpdateTask(taskElement, 1);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteTaskOK()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskMessage
            {
                Name = "Test Task",
                Description = "Test Description"
            };

            mockClient.Setup(x => x.DeleteTask(1));

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.DeleteTask(1);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteTaskBadRequest()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskMessage
            {
                Name = "",
                Description = "Test Description"
            };

            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.DeleteTask(0);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteTaskError500()
        {
            Mock<ILogger<TaskManagerController>> logger = new Mock<ILogger<TaskManagerController>>();
            Mock<ITaskManagerClient> mockClient = new Mock<ITaskManagerClient>();

            var taskElement = new TaskMessage
            {
                Name = "Element Name",
                Description = "Test Description"
            };

            mockClient
                .Setup(client => client.DeleteTask(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));


            TaskManagerController _controller = new TaskManagerController(logger.Object, mockClient.Object);

            var result = await _controller.DeleteTask(1);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, okResult.StatusCode);
        }
    }
}
