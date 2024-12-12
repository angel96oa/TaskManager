using Microsoft.AspNetCore.Identity;

namespace TaskManager.Identity
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string username, string password);

        Task<IdentityResult> RegisterUserAsync(string username, string password);
    }
}
