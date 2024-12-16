using Microsoft.AspNetCore.Identity;

namespace TaskManager.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
