using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Identity;

namespace TaskManager.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authenticationService;
        private readonly IUserRoleService _userRoleService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IUserRoleService userRoleService)
        {
            _logger = logger;
            _authenticationService = authService;
            _userRoleService = userRoleService;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] LoginDto login)
        {
            try
            {
                await _authenticationService.RegisterUserAsync(login.User, login.Password);
                _logger.LogInformation("Created user: {User}", login.User);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Error on creating for user: {User}", login.User);
                return Unauthorized("Invalid username or password.");
            }
        }

        [HttpGet("AdminRights")]
        public async Task<IActionResult> AsignAdminRole(string username, string role)
        {
            try
            {
                await _userRoleService.AssignRoleToUserAsync(username, role);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Error on creating for user: {User}", username);
                return Unauthorized("Invalid username or password.");
            }
        }
    }
}
