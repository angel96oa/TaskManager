using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Identity;

namespace TaskManager.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authenticationService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authenticationService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var token = await _authenticationService.AuthenticateAsync(login.User, login.Password);
                _logger.LogInformation("Login OK for user: {User}", login.User);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Error on login for user: {User}", login.User);
                return Unauthorized("Invalid username or password.");
            }
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] LoginDto login)
        {
            try
            {
                var IdentityResult = await _authenticationService.RegisterUserAsync(login.User, login.Password);
                _logger.LogInformation("Created user: {User}", login.User);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Error on creating for user: {User}", login.User);
                return Unauthorized("Invalid username or password.");
            }
        }
    }
}
