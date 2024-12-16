using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TaskManager.Identity
{
    public class UserRoleService : IUserRoleService, IDisposable
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserRoleService> _logger;
        private bool disposedValue;

        public UserRoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<UserRoleService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeRoles()
        {
            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                var role = new ApplicationRole { Name = "Admin" };
                await _roleManager.CreateAsync(role);
            }

            // Crear el rol "User" si no existe
            if (await _roleManager.FindByNameAsync("User") == null)
            {
                var role = new ApplicationRole { Name = "User" };
                await _roleManager.CreateAsync(role);
            }
        }

        public async Task AssignRoleToUserAsync(string username, string roleName)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ("User not found");
            }

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new Exception("Role not found");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                throw new Exception("Error assigning role");
            }

            _logger.LogInformation("Role asigned to user");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _userManager.Dispose();
                    _roleManager.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
