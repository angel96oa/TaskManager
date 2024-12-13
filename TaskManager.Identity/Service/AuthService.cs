using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
namespace TaskManager.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            return true;
        }

        public async Task<IdentityResult> RegisterUserAsync(string username, string password)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                PasswordHash = password
            };

            var result = await _userManager.CreateAsync(user, password);

            return result;
        }
    }

}
