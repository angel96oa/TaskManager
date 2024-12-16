using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

                    await authService.AuthenticateAsync(username, password);

                    var roles = await authService.GetRolesForUserAsync(username);
                    _logger.LogInformation("User authenticated: {User}", username);

                    var requiredRoles = context.GetEndpoint()?.Metadata.GetMetadata<ApplicationRole>()?.Name;
                    if (requiredRoles != null && !roles.Any(role => requiredRoles.Contains(role)))
                    {
                        throw new UnauthorizedAccessException("User does not have the required role.");
                    }

                    _logger.LogInformation("User authorized with roles: {Roles}", string.Join(", ", roles));
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
