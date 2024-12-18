using TaskManager.Messaging;
using TaskManager.Data;
using TaskManager.Service;
using TaskManager.Proto;
using Moq;
using Microsoft.Extensions.Logging;
using AutoFixture;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.UnitTest.Service
{
    public class TaskManagerServiceUnitTest
    {
        [Fact]
        public async Task CreateTask_Successful_ReturnsTaskIdMessage()
        {
            // Arrange

            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            var request = new TaskMessage { Name = "Test Task", Description = "Test Description", Status = Status.Open };
            var expectedId = 1;

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()))
                                .ReturnsAsync(expectedId);

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            var result = await _service.CreateTask(request, null);

            // Assert
            Assert.Equal(expectedId, result.Id);
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateTask_Error_RabbitMQException()
        {
            //Arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            var request = new TaskMessage { Name = "Test Task", Description = "Test Description", Status = Status.Open };
            var expectedId = 1;

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()))
                    .ReturnsAsync(expectedId);
            _mockRabbitMQService.Setup(rabbit => rabbit.SendMessage(It.IsAny<string>()))
                                .Throws(new Exception("Error sending message"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            await Assert.ThrowsAsync<Exception>(() => _service.CreateTask(request, null));

            //Assert
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);

        }

        [Fact]
        public async Task CreateTask_Error_DbUpdateException()
        {
            //Arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            var request = new TaskMessage { Name = "Test Task", Description = "Test Description", Status = Status.Open };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(db => db.CreateTaskAsync(It.IsAny<TaskElement>()))
                                .ThrowsAsync(new DbUpdateException("Error updating database"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            await Assert.ThrowsAsync<DbUpdateException>(() => _service.CreateTask(request, null));

            //Assert
            _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ReadTask_Successful_ReturnsTaskMessage()
        {
            // Arrange

            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskIdMessage request = new TaskIdMessage { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>())).ReturnsAsync(element);
            _mockRabbitMQService.Setup(rabbit => rabbit.SendMessage(It.IsAny<string>()));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            var result = await _service.ReadTask(request, null);

            // Assert
            Assert.Equal(element.Name, result.Name);
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(It.IsAny<int>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ReadTask_Error_RabbitMQException()
        {
            // Arrange

            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskIdMessage request = new TaskIdMessage { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>())).ReturnsAsync(element);
            _mockRabbitMQService.Setup(rabbit => rabbit.SendMessage(It.IsAny<string>()))
                .Throws(new Exception("Error sending message"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            await Assert.ThrowsAsync<Exception>(() => _service.ReadTask(request, null));

            // Assert

            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(It.IsAny<int>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ReadTask_Error_DbException()
        {
            // Arrange

            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskIdMessage request = new TaskIdMessage { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new DbUpdateException("Error sending message"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            await Assert.ThrowsAsync<DbUpdateException>(() => _service.ReadTask(request, null));

            // Assert

            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(It.IsAny<int>()), Times.Never);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTask_Successful()
        {
            //arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskMessageId request = new TaskMessageId { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>())).ReturnsAsync(new TaskResponseEmpty());
            _mockRabbitMQService.Setup(rabbit => rabbit.SendMessage(It.IsAny<string>()));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            var result = await _service.UpdateTask(request, null);

            // Assert
            Assert.NotNull(result);
            _mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_Error_RabbitMQException()
        {
            //arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskMessageId request = new TaskMessageId { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>())).ReturnsAsync(new TaskResponseEmpty());
            _mockRabbitMQService.Setup(rabbit => rabbit.SendMessage(It.IsAny<string>()))
                                .Throws(new Exception("Error sending message"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            await Assert.ThrowsAsync<Exception>(() => _service.UpdateTask(request, null));

            _mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTask_Error_DbException()
        {
            //arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskMessageId request = new TaskMessageId { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>()))
                .ThrowsAsync(new DbUpdateException("Error updating database"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            await Assert.ThrowsAsync<DbUpdateException>(() => _service.UpdateTask(request, null));

            _mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTask_Successful()
        {
            //arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskIdMessage request = new TaskIdMessage { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>())).ReturnsAsync(new TaskResponseEmpty());

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            // Act
            var result = await _service.DeleteTask(request, null);

            // Assert
            Assert.NotNull(result);
            _mockTaskRepository.Verify(repo => repo.DeleteTaskAsync(It.IsAny<int>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_Error_RabbitMQException()
        {
            //arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskMessageId request = new TaskMessageId { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>())).ReturnsAsync(new TaskResponseEmpty());
            _mockRabbitMQService.Setup(rabbit => rabbit.SendMessage(It.IsAny<string>()))
                                .Throws(new Exception("Error sending message"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            await Assert.ThrowsAsync<Exception>(() => _service.UpdateTask(request, null));

            _mockTaskRepository.Verify(repo => repo.UpdateTaskAsync(It.IsAny<TaskElement>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTask_Error_DbException()
        {
            //arrange
            var fixture = new Fixture();
            Mock<ILogger<TaskManagerService>> _mockLogger = new Mock<ILogger<TaskManagerService>>();
            Mock<ITaskRepository> _mockTaskRepository = new Mock<ITaskRepository>();
            Mock<IRabbitMQService> _mockRabbitMQService = new Mock<IRabbitMQService>();
            TaskIdMessage request = new TaskIdMessage { Id = 1 };
            TaskElement element = new TaskElement { Name = "Test Task", Description = "Test Description", status = Status.Open.ToString() };

            fixture.Customize<ILogger<RabbitMQService>>(c => c.FromFactory(() => Mock.Of<ILogger<RabbitMQService>>()));
            fixture.Customize<IOptions<RabbitMQConfiguration>>(c => c.FromFactory(() => Mock.Of<IOptions<RabbitMQConfiguration>>()));

            _mockTaskRepository.Setup(repo => repo.DeleteTaskAsync(It.IsAny<int>()))
                .ThrowsAsync(new DbUpdateException("Error updating database"));

            TaskManagerService _service = new TaskManagerService(_mockLogger.Object, _mockTaskRepository.Object, _mockRabbitMQService.Object);

            await Assert.ThrowsAsync<DbUpdateException>(() => _service.DeleteTask(request, null));

            _mockTaskRepository.Verify(repo => repo.DeleteTaskAsync(It.IsAny<int>()), Times.Once);
            _mockRabbitMQService.Verify(rabbit => rabbit.SendMessage(It.IsAny<string>()), Times.Never);
        }
    }
}