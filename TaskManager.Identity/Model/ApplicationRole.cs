
using Microsoft.AspNetCore.Identity;

namespace TaskManager.Identity
{
    public class ApplicationRole : IdentityRole
    {


        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
