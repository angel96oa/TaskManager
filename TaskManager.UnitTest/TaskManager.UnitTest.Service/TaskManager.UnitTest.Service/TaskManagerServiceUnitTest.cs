using TaskManager.Messaging;
using TaskManager.Data;
using TaskManager.Service;
using Moq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using TaskManager.Proto;

namespace TaskManager.UnitTest.Service
{
    public class TaskManagerServiceUnitTest
    {
        private readonly Mock<ILogger<TaskManagerService>> _mockLogger;
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<RabbitMQService> _mockRabbitMQService;
        private readonly TaskManagerService _service;

        public TaskManagerServiceUnitTest()
        {
            _mockLogger = new Mock<ILogger<TaskManagerService>>();
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockRabbitMQService = new Mock<RabbitMQService>();

            _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);
        }

        [Theory]
        [AutoMockData]
        public async Task CreateTask_Successful_ReturnsTaskIdMessage()
        {
            // Arrange
            var request = new TaskMessage { Name = "Test Task", Description = "Test Description", Status = Status.Open };
            var expectedId = 1;

            _mockTaskRepository.Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()))
                               .ReturnsAsync(expectedId);

            // Act
            var result = await _service.CreateTask(request, null);

            // Assert
            Assert.Equal(expectedId, result.Id);
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateTask_RabbitMQException_LogsError()
        {
            // Arrange
            var request = new TaskMessage { Name = "Test Task", Description = "Test Description", Status = Status.Open };
            var expectedId = 1;

            _mockTaskRepository.Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()))
                               .ReturnsAsync(expectedId);
            _mockRabbitMQService.Setup(rabbit => rabbit.SendMessage(It.IsAny<string>()))
                                .Throws(new Exception("Error"));

            // Act
            var result = await _service.CreateTask(request, null);

            // Assert
            Assert.Equal(expectedId, result.Id);
            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
        }

        [Fact]
        public async Task ReadTask_Successful_ReturnsTaskMessage()
        {
            // Arrange
            var request = new TaskIdMessage { Id = 1 };
            var taskElement = new TaskElement
            {
                Name = "Test Task",
                Description = "Test Description",
                status = Status.Open.ToString()
            };

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync(taskElement);

            // Act
            var result = await _service.ReadTask(request, null);

            // Assert
            Assert.Equal(taskElement.Name, result.Name);
            Assert.Equal(taskElement.Description, result.Description);
            Assert.Equal(Status.Open.ToString(), result.Status.ToString());

            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(It.IsAny<int>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_Successful_ReturnsEmptyResponse()
        {
            // Arrange
            var request = new TaskMessageId
            {
                Id = 1,
                Name = "Updated Task",
                Description = "Updated Description",
                Status = Status.Closed
            };

            var emptyResponse = new TaskResponseEmpty();
            _mockTaskRepository.Setup(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>()))
                               .ReturnsAsync(emptyResponse);

            // Act
            var result = await _service.UpdateTask(request, null);

            // Assert
            Assert.NotNull(result);
            _mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_Successful_ReturnsEmptyResponse()
        {
            // Arrange
            var request = new TaskIdMessage { Id = 1 };
            var emptyResponse = new TaskResponseEmpty();

            _mockTaskRepository.Setup(repo => repo.DeleteTaskAsync(It.IsAny<int>()))
                               .ReturnsAsync(emptyResponse);

            // Act
            var result = await _service.DeleteTask(request, null);

            // Assert
            Assert.NotNull(result);
            _mockTaskRepository.Verify(repo => repo.DeleteTaskAsync(It.IsAny<int>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }
    }
}