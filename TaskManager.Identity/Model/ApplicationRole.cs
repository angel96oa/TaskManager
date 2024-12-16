
using Microsoft.AspNetCore.Identity;

namespace TaskManager.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
