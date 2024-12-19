using Moq;
using TaskManager.API;
using TaskManager.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;

namespace TaskManager.UnitTest
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task CreateUserOK()
        {
            Mock<ILogger<AuthController>> logger = new Mock<ILogger<AuthController>>();
            Mock<IAuthService> authService = new Mock<IAuthService>();
            Mock<IUserRoleService> userRoleService = new Mock<IUserRoleService>();

            authService.Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>()));

            AuthController _controller = new AuthController(logger.Object, authService.Object, userRoleService.Object);
            LoginDto login = new LoginDto()
            {
                User = "test",
                Password = "test"
            };
            var result = await _controller.CreateUser(login);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task CreateUserUnauthorized()
        {
            Mock<ILogger<AuthController>> logger = new Mock<ILogger<AuthController>>();
            Mock<IAuthService> authService = new Mock<IAuthService>();
            Mock<IUserRoleService> userRoleService = new Mock<IUserRoleService>();

            authService.Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new UnauthorizedAccessException("Exception"));

            AuthController _controller = new AuthController(logger.Object, authService.Object, userRoleService.Object);
            LoginDto login = new LoginDto()
            {
                User = "test",
                Password = "test"
            };
            var result = await _controller.CreateUser(login);

            // Assert
            var okResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, okResult.StatusCode);
        }

        [Fact]
        public async Task AsignAdminRoleOK()
        {
            Mock<ILogger<AuthController>> logger = new Mock<ILogger<AuthController>>();
            Mock<IAuthService> authService = new Mock<IAuthService>();
            Mock<IUserRoleService> userRoleService = new Mock<IUserRoleService>();

            userRoleService.Setup(x => x.AssignRoleToUserAsync(It.IsAny<string>(), It.IsAny<string>()));

            AuthController _controller = new AuthController(logger.Object, authService.Object, userRoleService.Object);

            var result = await _controller.AsignAdminRole("user", "admin");

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task AsignAdminRoleUnauthorized()
        {
            Mock<ILogger<AuthController>> logger = new Mock<ILogger<AuthController>>();
            Mock<IAuthService> authService = new Mock<IAuthService>();
            Mock<IUserRoleService> userRoleService = new Mock<IUserRoleService>();

            userRoleService.Setup(x => x.AssignRoleToUserAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new UnauthorizedAccessException("Exception"));

            AuthController _controller = new AuthController(logger.Object, authService.Object, userRoleService.Object);
            LoginDto login = new LoginDto()
            {
                User = "test",
                Password = "test"
            };
            var result = await _controller.AsignAdminRole("user", "admin");

            // Assert
            var okResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, okResult.StatusCode);
        }
    }
}