using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;

namespace TaskManager.Identity
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BasicAuthMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BasicAuthMiddleware(RequestDelegate next, ILogger<BasicAuthMiddleware> logger, IServiceScopeFactory serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceScopeFactory = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
                {
                    throw new UnauthorizedAccessException("Authorization header is missing or invalid.");
                }

                var authValue = authHeader.Substring(6);
                var credentialBytes = Convert.FromBase64String(authValue);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                var username = credentials[0];
                var password = credentials[1];

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                    var isAuthenticated = await authService.AuthenticateAsync(username, password);
                    if (!isAuthenticated)
                    {
                        throw new UnauthorizedAccessException("Invalid credentials.");
                    }

                    var roles = await authService.GetRolesForUserAsync(username);
                    if (!roles.Contains("Admin"))
                    {
                        throw new UnauthorizedAccessException("User does not have the required role.");
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username)
                    };
                    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                    var identity = new ClaimsIdentity(claims, "Basic");
                    context.User = new ClaimsPrincipal(identity);
                }
            
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Authentication failed");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid credentials");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal Server Error");
                return;
            }

            await _next(context);
        }
    }
}
